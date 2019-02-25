namespace GetIt

open System
open System.Diagnostics
open System.IO
open System.IO.Pipes
open System.Reactive.Linq
open System.Threading
open System.Threading.Tasks
open FSharp.Control.Reactive
open Thoth.Json.Net

type EventHandler =
    | KeyDown of key: KeyboardKey option * handler: (KeyboardKey -> unit)
    | ClickScene of handler: (Position -> MouseButton -> unit)
    | ClickPlayer of playerId: PlayerId * handler: (unit -> unit)
    | MouseEnterPlayer of playerId: PlayerId * handler: (unit -> unit)

type Model =
    { SceneBounds: Rectangle
      Players: Map<PlayerId, PlayerData>
      MouseState: MouseState
      KeyboardState: KeyboardState
      EventHandlers: EventHandler list }

module Model =
    let mutable current =
        { SceneBounds = { Position = Position.zero; Size = { Width = 0.; Height = 0. } }
          Players = Map.empty
          MouseState = MouseState.empty
          KeyboardState = KeyboardState.empty
          EventHandlers = [] }

module internal InterProcessCommunication =
    let private uiProcess =
        lazy (
            // TODO determine UI technology based on host OS
            let startInfo =
#if DEBUG
                let path =
                    let rec parentPaths path acc =
                        if isNull path then List.rev acc
                        else parentPaths (Path.GetDirectoryName path) (path :: acc)
                    parentPaths (Path.GetFullPath ".") []
                    |> Seq.choose (fun p ->
                        let projectDir = Path.Combine(p, "GetIt.WPF")
                        if Directory.Exists projectDir
                        then Some projectDir
                        else None
                    )
                    |> Seq.head
                ProcessStartInfo("dotnet", sprintf "run --project %s" path)
#else
                ProcessStartInfo("GetIt.WPF.exe")
#endif
            
            let proc = Process.Start(startInfo)

            let pipeClient = new NamedPipeClientStream(".", "GetIt", PipeDirection.InOut, PipeOptions.Asynchronous)
            pipeClient.Connect()

            let reader = new StreamReader(pipeClient)
            let receiveObservable = MessageProcessing.getMessages reader UIToControllerMsg.decode

            let subscription =
                receiveObservable
                |> Observable.subscribe(fun (IdentifiableMsg (msgId, msg)) ->
                    // TODO handle events etc.
                    ()
                )

            (new StreamWriter(pipeClient), receiveObservable)
        )

    let private updatePlayer model playerId fn =
        let player = Map.find playerId model.Players |> fn
        { model with Players = Map.add playerId player model.Players }

    let private applyUIToControllerMessage message model =
        let updatePlayer = updatePlayer model
        match message with
        | UIToControllerMsg.MsgProcessed -> model
        | InitializedScene sceneBounds -> { model with SceneBounds = sceneBounds }

    let private applyControllerToUIMessage message model =
        let updatePlayer = updatePlayer model
        match message with
        | ControllerToUIMsg.MsgProcessed -> model
        | ShowScene -> model
        | AddPlayer (playerId, player) -> { model with Players = Map.add playerId player model.Players }
        | RemovePlayer playerId ->
            { model with Players = Map.remove playerId model.Players }
        | SetPosition (playerId, position) ->
            updatePlayer playerId (fun p -> { p with Position = position })
        | SetDirection (playerId, angle) ->
            updatePlayer playerId (fun p -> { p with Direction = angle })
        | SetSpeechBubble (playerId, speechBubble) ->
            updatePlayer playerId (fun p -> { p with SpeechBubble = speechBubble })
        | SetPen (playerId, pen) ->
            updatePlayer playerId (fun p -> { p with Pen = pen })
        | SetSizeFactor (playerId, sizeFactor) ->
            updatePlayer playerId (fun p -> { p with SizeFactor = sizeFactor })
        | SetNextCostume playerId ->
            updatePlayer playerId Player.nextCostume

    let sendCommand command =
        let (pipeWriter, receiveObservable) = uiProcess.Force()

        let msgId = Guid.NewGuid()

        use waitHandle = new ManualResetEventSlim()
        let mutable response = None

        use subscription =
            receiveObservable
            |> Observable.filter (fun (IdentifiableMsg (mId, msg)) -> mId = msgId)
            |> Observable.first
            |> Observable.subscribeWithCallbacks
                (fun (IdentifiableMsg (mId, msg)) ->
                    response <- Some (Ok msg)
                    waitHandle.Set()
                )
                (fun e ->
                    response <- Some (Error e)
                    waitHandle.Set()
                )
                (fun () ->
                    waitHandle.Set()
                )

        let line = Encode.toString 0 (ControllerToUIMsg.encode msgId command)
        pipeWriter.WriteLine(line)
        pipeWriter.Flush()
        
        waitHandle.Wait()

        match response with
        | Some (Ok msg) ->
            Model.current <-
                Model.current
                |> applyControllerToUIMessage command
                |> applyUIToControllerMessage  msg
        | Some (Error e) ->
            failwithf "Error while waiting for response: %O" e
        | None ->
            // Close the application if the UI has been closed (throwing an exception might be confusing)
            Environment.Exit 1

type Player(playerId) =
    let mutable isDisposed = 0

    member internal x.PlayerId with get() = playerId
    member private x.Player with get() = Map.find playerId Model.current.Players
    /// <summary>
    /// The actual size of the player.
    /// </summary>
    member x.Size with get() = x.Player.Size

    /// <summary>
    /// The factor that is used to resize the player.
    /// </summary>
    member x.SizeFactor with get() = x.Player.SizeFactor

    /// <summary>
    /// The position of the player.
    /// </summary>
    member x.Position with get() = x.Player.Position

    /// <summary>
    /// The actual bounds of the player.
    /// </summary>
    member x.Bounds with get() = x.Player.Bounds

    /// <summary>
    /// The direction of the player.
    /// </summary>
    member x.Direction with get() = x.Player.Direction

    /// <summary>
    /// The pen of the player.
    /// </summary>
    member x.Pen with get() = x.Player.Pen

    abstract member Dispose: unit -> unit
    default x.Dispose() =
        if Interlocked.Exchange(&isDisposed, 1) = 0
        then
            InterProcessCommunication.sendCommand (RemovePlayer playerId)

    interface IDisposable with
        member x.Dispose() = x.Dispose()

module Game =
    let mutable defaultTurtle = None

    type DefaultPlayer(playerId) =
        inherit Player(playerId)
        override x.Dispose() =
            base.Dispose()
            defaultTurtle <- None

    [<CompiledName("ShowSceneAndAddTurtle")>]
    let showSceneAndAddTurtle() =
        InterProcessCommunication.sendCommand ShowScene
        InterProcessCommunication.sendCommand (AddPlayer (PlayerId.create (), Player.turtle))
        defaultTurtle <-
            Map.toSeq Model.current.Players
            |> Seq.tryHead
            |> Option.map (fst >> (fun playerId -> new DefaultPlayer(playerId) :> Player ))
