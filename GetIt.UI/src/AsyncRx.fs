module AsyncRx

open Browser.Types
open Fable.Core.JsInterop
open FSharp.Control

let private transformAsync mapNextAsync (source: IAsyncObservable<_>) =
    let subscribeAsync (aobv : IAsyncObserver<'TResult>) =
        { new IAsyncObserver<'TSource> with
            member __.OnNextAsync x = mapNextAsync aobv.OnNextAsync x
            member __.OnErrorAsync err = aobv.OnErrorAsync err
            member __.OnCompletedAsync () = aobv.OnCompletedAsync ()
        }
        |> source.SubscribeAsync
    AsyncRx.create subscribeAsync

let skip n =
    let mutable remaining = n
    transformAsync (fun onNextAsync item -> async {
        if remaining <= 0 then
            do! onNextAsync item
        else
            remaining <- remaining - 1
    })

let buffer n =
    let items = ResizeArray<_>()
    transformAsync (fun onNextAsync item -> async {
        if items.Count = n then
            items.RemoveAt 0
        items.Add item
        if items.Count = n then
            do! onNextAsync (Seq.toArray items)
    })

let pairwise source =
    source
    |> buffer 2
    |> AsyncRx.map (fun b -> b.[0], b.[1])

let sampleWith (sampler: IAsyncObservable<_>) (source: IAsyncObservable<_>) =
    AsyncRx.create (fun observer -> async {
        let mutable current = None
        let! d1 = source.SubscribeAsync(fun notification -> async {
            match notification with
            | OnNext item -> current <- Some item
            | OnError e -> ()
            | OnCompleted -> ()
        })
        let! d2 = sampler.SubscribeAsync(fun item -> async {
            match current with
            | Some x ->
                current <- None
                do! observer.OnNextAsync x
            | None -> ()
        })
        return AsyncDisposable.Composite [ d1; d2 ]
    })

let repeat (source: IAsyncObservable<_>) =
    AsyncRx.create (fun observer -> async {
        let mutable subscription = AsyncDisposable.Empty
        let rec subscribe notification = async {
            match notification with
            | OnNext item -> do! observer.OnNextAsync item
            | OnError e -> do! observer.OnErrorAsync e
            | OnCompleted ->
                let! d = source.SubscribeAsync(subscribe)
                subscription <- d
        }
        let! d = source.SubscribeAsync(subscribe)
        subscription <- d
        return AsyncDisposable.Create(fun () -> subscription.DisposeAsync())
    })

let ``finally`` fn (source: IAsyncObservable<_>) =
    AsyncRx.create (fun observer -> async {
        return! source.SubscribeAsync(fun notification -> async {
            match notification with
            | OnNext item -> do! observer.OnNextAsync item
            | OnError e ->
                do! observer.OnErrorAsync e
                do! fn ()
            | OnCompleted ->
                do! observer.OnCompletedAsync()
                do! fn ()
        })
    })

let observeSubtreeMutations (parent: Node) =
    AsyncRx.create (fun obs -> async {
        let onMutate mutations =
            mutations
            |> Seq.iter (obs.OnNextAsync >> Async.StartImmediate)
        let mutationObserver = createNew Browser.Dom.window?MutationObserver (onMutate)
        let mutationObserverConfig = createObj [
            "childList" ==> true
            "characterData" ==> true
            "subtree" ==> true
        ]
        mutationObserver?observe(parent, mutationObserverConfig)
        return AsyncDisposable.Create (fun () -> async {
            mutationObserver?disconnect()
        })
    })

let observeSubTreeAdditions parent : IAsyncObservable<Node> =
    observeSubtreeMutations parent
    |> AsyncRx.flatMap (fun m -> m?addedNodes |> AsyncRx.ofSeq)

let observeSubTreeTextChanged parent : IAsyncObservable<Text> =
    observeSubtreeMutations parent
    |> AsyncRx.filter (fun m -> m?``type`` = "characterData")
    |> AsyncRx.map (fun m -> m?target)

let observeElementSizeFromWindowResize selector =
    AsyncRx.create (fun obs -> async {
        let resizeCanvas evt =
            obs.OnNextAsync ()
            |> Async.StartImmediate
            ()
        resizeCanvas ()
        Browser.Dom.window.addEventListener("resize", resizeCanvas, false)
        return AsyncDisposable.Create (fun () -> async {
            Browser.Dom.window.removeEventListener("resize", resizeCanvas, false)
        })
    })
    |> AsyncRx.choose (fun () ->
        match Browser.Dom.document.querySelector selector :?> HTMLElement |> Option.ofObj with
        | Some element -> Some (element.offsetWidth, element.offsetHeight)
        | None -> None
    )

let requestAnimationFrameObservable =
    AsyncRx.create (fun observer -> async {
        Browser.Dom.window.requestAnimationFrame (fun timestamp ->
            async {
                do! observer.OnNextAsync ()
                do! observer.OnCompletedAsync()
            }
            |> Async.StartImmediate
        )
        |> ignore
        return AsyncDisposable.Empty
    })
    |> repeat

let beforeWindowUnloadObservable =
    AsyncRx.create (fun observer -> async {
        let listener e =
            async {
                do! observer.OnNextAsync ()
                do! observer.OnCompletedAsync()
            }
            |> Async.StartImmediate

        Browser.Dom.window.addEventListener ("beforeunload", listener) |> ignore
        return AsyncDisposable.Create(fun () -> async { Browser.Dom.window.removeEventListener("beforeunload", listener) })
    })

let ignore source =
    source
    |> AsyncRx.flatMapLatest (ignore >> AsyncRx.empty)
