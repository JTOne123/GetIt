﻿namespace GetIt

open System
open System.IO
open System.IO.Pipes
open System.Reactive.Concurrency
open System.Windows
open System.Windows.Media
open System.Windows.Media.Imaging
open System.Windows.Threading
open FSharp.Control.Reactive
open Xamarin.Forms
open Xamarin.Forms.Platform.WPF
open Xamarin.Forms.Platform.WPF.Helpers
open GetIt.Windows

[<assembly: ExportRenderer(typeof<SkiaSharp.Views.Forms.SKCanvasView>, typeof<SkiaSharp.Wpf.SKCanvasViewRenderer>)>]
do ()

type MainWindow() =
    inherit FormsApplicationPage()

module Main =
    let private eventSubject = new System.Reactive.Subjects.Subject<UIEvent>()

    let private tryGetPositionOnSceneControl positionOnScreen =
        System.Windows.Application.Current.Dispatcher.Invoke(fun () ->
            if isNull System.Windows.Application.Current.MainWindow then None
            else
                let window = System.Windows.Application.Current.MainWindow :?> MainWindow

                // TODO simplify if https://github.com/xamarin/Xamarin.Forms/issues/5921 is resolved
                TreeHelper.FindChildren<Xamarin.Forms.Platform.WPF.Controls.FormsNavigationPage>(window, forceUsingTheVisualTreeHelper = true)
                |> Seq.tryHead
                |> Option.bind (fun navigationPage ->
                    TreeHelper.FindChildren<FormsPanel>(navigationPage, forceUsingTheVisualTreeHelper = true)
                    |> Seq.filter (fun p -> p.Element.AutomationId = "scene")
                    |> Seq.tryHead
                )
                |> Option.bind (fun scene ->
                    try
                        let virtualDesktopLeft = Win32.GetSystemMetrics(Win32.SystemMetric.SM_XVIRTUALSCREEN)
                        let virtualDesktopTop = Win32.GetSystemMetrics(Win32.SystemMetric.SM_YVIRTUALSCREEN)
                        let virtualDesktopWidth = Win32.GetSystemMetrics(Win32.SystemMetric.SM_CXVIRTUALSCREEN)
                        let virtualDesktopHeight = Win32.GetSystemMetrics(Win32.SystemMetric.SM_CYVIRTUALSCREEN)

                        let screenPoint =
                            System.Windows.Point(
                                float virtualDesktopWidth * positionOnScreen.X + float virtualDesktopLeft,
                                float virtualDesktopHeight * positionOnScreen.Y + float virtualDesktopTop
                            )
                    
                        let scenePoint = scene.PointFromScreen(screenPoint)
                        Some { X = scenePoint.X; Y = scenePoint.Y }
                    with _ -> None
            )
        )

    let private windowIcon =
        use stream = typeof<GetIt.App>.Assembly.GetManifestResourceStream("GetIt.UI.icon.png")
        let bitmap = BitmapImage()
        bitmap.BeginInit()
        bitmap.StreamSource <- stream
        bitmap.CacheOption <- BitmapCacheOption.OnLoad
        bitmap.EndInit()
        bitmap.Freeze()
        bitmap

    let private doWithSceneControl fn =
        let rec execute retries =
            if retries = 0 then failwith "Can't execute function with scene control: No more retries left."
            else
                try
                    System.Windows.Application.Current.Dispatcher.Invoke(fun () ->
                        if isNull System.Windows.Application.Current.MainWindow then failwith "No main window"
                        else
                            let window = System.Windows.Application.Current.MainWindow :?> MainWindow

                            // TODO simplify if https://github.com/xamarin/Xamarin.Forms/issues/5921 is resolved
                            TreeHelper.FindChildren<Xamarin.Forms.Platform.WPF.Controls.FormsNavigationPage>(window, forceUsingTheVisualTreeHelper = true)
                            |> Seq.tryHead
                            |> Option.bind (fun navigationPage ->
                                TreeHelper.FindChildren<FormsPanel>(navigationPage, forceUsingTheVisualTreeHelper = true)
                                |> Seq.filter (fun p -> p.Element.AutomationId = "scene")
                                |> Seq.tryHead
                            )
                            |> function
                            | Some sceneControl -> fn (sceneControl :> FrameworkElement)
                            | None -> failwith "Scene control not found"
                    )
                with e ->
                    printfn "Executing function with scene control failed: %s (Retries: %d)" e.Message retries
                    System.Threading.Thread.Sleep(100)
                    execute (retries - 1)
        execute 50

    let controlToImage (control: FrameworkElement) =
        let renderTargetBitmap = RenderTargetBitmap(int control.ActualWidth, int control.ActualHeight, 96., 96., PixelFormats.Pbgra32)
        renderTargetBitmap.Render control
        let encoder = PngBitmapEncoder()
        encoder.Frames.Add(BitmapFrame.Create renderTargetBitmap)
        use stream = new MemoryStream()
        encoder.Save stream
        stream.ToArray() |> PngImage

    let executeCommand cmd =
        match cmd with
        | UIMsgProcessed -> None
        | ShowScene windowSize ->
            let start onStarted onClosed =
                let app = System.Windows.Application()
                app.Exit.Subscribe(fun args -> onClosed()) |> ignore
                Forms.Init()
                let window = MainWindow()
                match windowSize with
                | SpecificSize size ->
                    window.Width <- size.Width
                    window.Height <- size.Height
                | Maximized ->
                    window.WindowState <- WindowState.Maximized
                window.Title <- "Get It"
                window.Icon <- windowIcon
                window.LoadApplication(GetIt.App eventSubject.OnNext)
                onStarted()
                app.Run(window)
            GetIt.App.showScene start
            // TODO remove if https://github.com/xamarin/Xamarin.Forms/issues/5910 is resolved
            doWithSceneControl (fun sceneControl -> sceneControl.ClipToBounds <- true)
            Some ControllerMsgProcessed
        | SetBackground background ->
            GetIt.App.setBackground background
            Some ControllerMsgProcessed
        | ClearScene ->
            GetIt.App.clearScene ()
            Some ControllerMsgProcessed
        | MakeScreenshot ->
            let sceneImage =
                System.Windows.Application.Current.Dispatcher.Invoke(
                    (fun () -> controlToImage System.Windows.Application.Current.MainWindow),
                    DispatcherPriority.ApplicationIdle // ensure rendering happened
                )
            Some (UIEvent (Screenshot sceneImage))
        | AddPlayer (playerId, player) ->
            GetIt.App.addPlayer playerId player
            Some ControllerMsgProcessed
        | RemovePlayer playerId ->
            GetIt.App.removePlayer playerId
            Some ControllerMsgProcessed
        | SetPosition (playerId, position) ->
            GetIt.App.setPosition playerId position
            Some ControllerMsgProcessed
        | SetDirection (playerId, angle) ->
            GetIt.App.setDirection playerId angle
            Some ControllerMsgProcessed
        | SetSpeechBubble (playerId, speechBubble) ->
            GetIt.App.setSpeechBubble playerId speechBubble
            Some ControllerMsgProcessed
        | SetPen (playerId, pen) ->
            GetIt.App.setPen playerId pen
            Some ControllerMsgProcessed
        | SetSizeFactor (playerId, sizeFactor) ->
            GetIt.App.setSizeFactor playerId sizeFactor
            Some ControllerMsgProcessed
        | SetNextCostume playerId ->
            GetIt.App.setNextCostume playerId
            Some ControllerMsgProcessed
        | ControllerEvent (KeyDown key) ->
            Some ControllerMsgProcessed
        | ControllerEvent (KeyUp key) ->
            Some ControllerMsgProcessed
        | ControllerEvent (MouseMove position) ->
            tryGetPositionOnSceneControl position
            |> Option.iter GetIt.App.setMousePosition
            Some ControllerMsgProcessed
        | ControllerEvent (MouseClick (mouseButton, position)) ->
            tryGetPositionOnSceneControl position
            |> Option.iter (GetIt.App.applyMouseClick mouseButton)
            Some ControllerMsgProcessed

    [<EntryPoint>]
    let main(_args) =
        // System.Diagnostics.Debugger.Launch() |> ignore

        while true do
            try
                use pipeServer =
                    new NamedPipeServerStream(
                        "GetIt",
                        PipeDirection.InOut,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous)
                pipeServer.WaitForConnection()

                let subject = MessageProcessing.forStream pipeServer UIToControllerMsg.encode ControllerToUIMsg.decode

                use eventSubscription =
                    eventSubject
                    |> Observable.map (fun evt -> IdentifiableMsg (Guid.NewGuid(), UIEvent evt))
                    |> Observable.observeOn ThreadPoolScheduler.Instance
                    |> Observable.subscribe subject.OnNext

                subject
                |> Observable.toEnumerable
                |> Seq.iter (fun (IdentifiableMsg (mId, msg)) ->
                    executeCommand msg
                    |> Option.map (fun response -> IdentifiableMsg (mId, response))
                    |> Option.iter subject.OnNext
                )
            with
            | e -> eprintfn "=== Unexpected exception: %O" e

        0
