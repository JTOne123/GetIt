using System;
using System.Reactive.Disposables;
using System.Threading;
using Elmish.Net;
using GetIt.Internal;
using LanguageExt;
using static LanguageExt.Prelude;

namespace GetIt
{
    public static class PlayerExtensions
    {
        public static void MoveTo(this PlayerOnScene player, double x, double y)
        {
            Game.DispatchMessageAndWaitForUpdate(new Message.SetPosition(player.Id, new Position(x, y)));
        }
        public static void MoveToCenter(this PlayerOnScene player) => player.MoveTo(0, 0);
        public static void Move(this PlayerOnScene player, double deltaX, double deltaY)
        {
            player.MoveTo(player.Position.X + deltaX, player.Position.Y + deltaY);
        }

        public static void MoveRight(this PlayerOnScene player, double steps) => player.Move(steps, 0);

        public static void MoveLeft(this PlayerOnScene player, double steps) => player.Move(-steps, 0);

        public static void MoveUp(this PlayerOnScene player, double steps) => player.Move(0, steps);

        public static void MoveDown(this PlayerOnScene player, double steps) => player.Move(0, -steps);

        public static void MoveInDirection(this PlayerOnScene player, double steps)
        {
            var directionRadians = player.Direction.Value / 180 * Math.PI;
            player.Move(
                Math.Cos(directionRadians) * steps,
                Math.Sin(directionRadians) * steps
            );
        }

        private static Random rand = new Random();
        public static void MoveToRandomPosition(this PlayerOnScene player)
        {
            var x = rand.Next((int)Game.State.SceneBounds.Left, (int)Game.State.SceneBounds.Right);
            var y = rand.Next((int)Game.State.SceneBounds.Bottom, (int)Game.State.SceneBounds.Top);
            player.MoveTo(x, y);
        }

        public static void SetDirection(this PlayerOnScene player, Degrees angle)
        {
            Game.DispatchMessageAndWaitForUpdate(new Message.SetDirection(player.Id, angle));
        }

        public static void RotateClockwise(this PlayerOnScene player, Degrees angle)
        {
            player.SetDirection(player.Direction - angle);
        }

        public static void RotateCounterClockwise(this PlayerOnScene player, Degrees angle)
        {
            player.SetDirection(player.Direction + angle);
        }

        public static void BounceIfOnEdge(this PlayerOnScene player)
        {
            if(player.Bounds.Top > Game.State.SceneBounds.Top
                || player.Bounds.Bottom < Game.State.SceneBounds.Bottom)
            {
                player.SetDirection(360 - player.Direction);
            }
            else if(player.Bounds.Right > Game.State.SceneBounds.Right
                || player.Bounds.Left < Game.State.SceneBounds.Left)
            {
                player.SetDirection(180 - player.Direction);
            }
        }

        public static void TurnUp(this PlayerOnScene player) => player.SetDirection(90);

        public static void TurnRight(this PlayerOnScene player) => player.SetDirection(0);

        public static void TurnDown(this PlayerOnScene player) => player.SetDirection(270);

        public static void TurnLeft(this PlayerOnScene player) => player.SetDirection(180);

        public static void Say(this PlayerOnScene player, string text)
        {
            Game.DispatchMessageAndWaitForUpdate(new Message.SetSpeechBubble(player.Id, new SpeechBubble.Say(text)));
        }

        public static void ShutUp(this PlayerOnScene player) =>
            Game.DispatchMessageAndWaitForUpdate(new Message.SetSpeechBubble(player.Id, None));

        public static void Say(this PlayerOnScene player, string text, double durationInSeconds)
        {
            player.Say(text);
            Game.Sleep(TimeSpan.FromSeconds(durationInSeconds).TotalMilliseconds);
            player.ShutUp();
        }

        public static string Ask(this PlayerOnScene player, string question)
        {
            using (var signal = new ManualResetEventSlim())
            {
                string answer = null;
                Game.DispatchMessageAndWaitForUpdate(
                    new Message.SetSpeechBubble(
                        player.Id,
                        new SpeechBubble.Ask(question, "", a => { answer = a; signal.Set(); })));
                signal.Wait();
                return answer;
            }
        }

        public static void SetPen(this PlayerOnScene player, Pen pen)
        {
            Game.DispatchMessageAndWaitForUpdate(new Message.SetPen(player.Id, pen));
        }

        public static void TurnOnPen(this PlayerOnScene player) => player.SetPen(player.Pen.With(p => p.IsOn, true));

        public static void TurnOffPen(this PlayerOnScene player) => player.SetPen(player.Pen.With(p => p.IsOn, false));

        public static void TogglePenOnOff(this PlayerOnScene player) => player.SetPen(player.Pen.With(p => p.IsOn, !player.Pen.IsOn));

        public static void SetPenColor(this PlayerOnScene player, RGBA color) => player.SetPen(player.Pen.With(p => p.Color, color));

        public static void ShiftPenColor(this PlayerOnScene player, double shift) => player.SetPen(player.Pen.WithHueShift(shift));

        public static void SetPenWeight(this PlayerOnScene player, double weight) => player.SetPen(player.Pen.With(p => p.Weight, weight));

        public static void SetSizeFactor(this PlayerOnScene player, double sizeFactor)
        {
            Game.DispatchMessageAndWaitForUpdate(new Message.SetSizeFactor(player.Id, sizeFactor));
        }

        public static void ChangeSizeFactor(this PlayerOnScene player, double change) => player.SetSizeFactor(player.SizeFactor + change);

        public static void ChangePenWeight(this PlayerOnScene player, double change) => player.SetPenWeight(player.Pen.Weight + change);

        public static Degrees GetDirectionToMouse(this PlayerOnScene player) => player.Position.AngleTo(Game.State.Mouse.Position);

        public static double GetDistanceToMouse(this PlayerOnScene player) => player.Position.DistanceTo(Game.State.Mouse.Position);

        private static IDisposable OnKeyDown(this PlayerOnScene player, Option<KeyboardKey> key, Action<KeyboardKey> action)
        {
            var handler = new EventHandler.KeyDown(key, action);
            return Game.AddEventHandler(handler);
        }

        public static IDisposable OnKeyDown(this PlayerOnScene player, KeyboardKey key, Action<PlayerOnScene> action)
        {
            return player.OnKeyDown(Some(key), _ => action(player));
        }

        public static IDisposable OnAnyKeyDown(this PlayerOnScene player, Action<PlayerOnScene, KeyboardKey> action)
        {
            return player.OnKeyDown(None, key => action(player, key));
        }

        public static IDisposable OnMouseEnter(this PlayerOnScene player, Action<PlayerOnScene> action)
        {
            var handler = new EventHandler.MouseEnterPlayer(player.Id, () => action(player));
            return Game.AddEventHandler(handler);
        }

        public static IDisposable OnClick(this PlayerOnScene player, Action<PlayerOnScene> action)
        {
            var handler = new EventHandler.ClickPlayer(player.Id, () => action(player));
            return Game.AddEventHandler(handler);
        }
    }
}