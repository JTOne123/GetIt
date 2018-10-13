﻿namespace GetIt.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Program1();
            // Program2();
            // Program3();
            // Program4();
            // Program5();
            // Program6();
            // Program7();
            // Program8();
            // Program9();
            // Program10();
            // Program11();
            // Program12();
            Program13();
        }

        private static void Program1()
        {
            Game.ShowSceneAndAddTurtle();

            Turtle.GoTo(0, 0);
            Turtle.SetPenWeight(1.5);
            Turtle.SetPenColor(RGBColor.Cyan);
            Turtle.TurnOnPen();
            var n = 5;
            while (n < 400)
            {
                Turtle.Go(n);
                Turtle.RotateCounterClockwise(89.5);

                Turtle.ShiftPenColor(10.0 / 360);
                n++;

                Game.Sleep(10);
            }
        }

        private static void Program2()
        {
            Game.ShowSceneAndAddTurtle();

            Turtle.GoTo(0, 0);
            for (int i = 0; i < 36; i++)
            {
                Turtle.RotateClockwise(10);
                Turtle.Go(10);
                Game.Sleep(50);
            }
        }

        private static void Program3()
        {
            Game.ShowSceneAndAddTurtle();

            Turtle.GoTo(0, 0);
            Turtle.Say("Let's do it", 2);
            for (var i = 0; i < 10; i++)
            {
                Turtle.Go(10);
                Game.Sleep(50);
            }
            Turtle.Say("Nice one");
            for (var i = 0; i < 10; i++)
            {
                Turtle.Go(-10);
                Game.Sleep(50);
            }
            Turtle.ShutUp();
            for (var i = 0; i < 10; i++)
            {
                Turtle.Go(10);
                Game.Sleep(50);
            }
            Turtle.Say("Done");
        }

        private static void Program4()
        {
            Game.ShowSceneAndAddTurtle();

            Turtle.GoTo(0, 0);
            Turtle.SetPenWeight(1.5);
            Turtle.SetPenColor(RGBColor.Cyan);
            Turtle.TurnOnPen();
            for (var i = 0; i < 10; i++)
            {
                Turtle.Go(10);
                Game.Sleep(50);
            }
            Game.ClearScene();
            for (var i = 0; i < 10; i++)
            {
                Turtle.Go(-10);
                Game.Sleep(50);
            }
            Game.ClearScene();
            for (var i = 0; i < 10; i++)
            {
                Turtle.Go(10);
                Game.Sleep(50);
            }
            Game.ClearScene();
        }

        private static void Program5()
        {
            Game.ShowSceneAndAddTurtle();

            Turtle.Say("Move me with arrow keys");
            using (Turtle.OnKeyDown(Models.KeyboardKey.Up, player => player.ShutUp()))
            using (Turtle.OnKeyDown(Models.KeyboardKey.Down, player => player.ShutUp()))
            using (Turtle.OnKeyDown(Models.KeyboardKey.Left, player => player.ShutUp()))
            using (Turtle.OnKeyDown(Models.KeyboardKey.Right, player => player.ShutUp()))
            using (Turtle.OnKeyDown(Models.KeyboardKey.Up, player => player.MoveUp(10)))
            using (Turtle.OnKeyDown(Models.KeyboardKey.Down, player => player.MoveDown(10)))
            using (Turtle.OnKeyDown(Models.KeyboardKey.Left, player => player.MoveLeft(10)))
            using (Turtle.OnKeyDown(Models.KeyboardKey.Right, player => player.MoveRight(10)))
            {
                Game.Sleep(5000);
            }
            Turtle.Say("Game over");
        }

        private static void Program6()
        {
            Game.ShowSceneAndAddTurtle();

            Turtle.OnMouseEnter(player => player.GoToRandomPosition());
        }

        private static void Program7()
        {
            Game.ShowSceneAndAddTurtle();

            Turtle.Say("Try and hit me, sucker!", 2);
            Turtle.OnClick(player => player.Say("Ouch, that hurts!", 2));
        }

        private static void Program8()
        {
            Game.ShowSceneAndAddTurtle();

            for (int i = 0; i < 500; i++)
            {
                Turtle.Say(new string('A', i));
                Game.Sleep(20);
            }
        }

        private static void Program9()
        {
            Game.ShowSceneAndAddTurtle();

            Turtle.SetPenWeight(5);

            Turtle.TurnOnPen();
            Turtle.GoTo(33, 33);
            Game.Sleep(100);
            Turtle.TurnOffPen();
            Turtle.GoTo(66, 66);
            Game.Sleep(100);
            Turtle.TurnOnPen();
            Turtle.GoTo(100, 100);
            Game.Sleep(100);

            Turtle.GoTo(100, 33);
            Game.Sleep(100);
            Turtle.TurnOffPen();
            Turtle.GoTo(100, -33);
            Game.Sleep(100);
            Turtle.TurnOnPen();
            Turtle.GoTo(100, -100);
            Game.Sleep(100);
            
            Turtle.GoTo(66, -66);
            Game.Sleep(100);
            Turtle.TurnOffPen();
            Turtle.GoTo(33, -33);
            Game.Sleep(100);
            Turtle.TurnOnPen();
            Turtle.GoToCenter();
        }

        private static void Program10()
        {
            Game.ShowSceneAndAddTurtle();

            Turtle.TurnOnPen();
            Turtle.SetPenColor(RGBColor.Red);
            while (Turtle.GetDistanceToMouse() > 10)
            {
                Turtle.ShiftPenColor(10.0 / 360);
                var direction = Turtle.GetDirectionToMouse();
                Turtle.SetDirection(direction);
                Turtle.Go(10);
                Game.Sleep(50);
            }
            Turtle.Say("Geschnappt :-)");
        }

        private static void Program11()
        {
            Game.ShowSceneAndAddTurtle();

            Turtle.TurnOnPen();
            Turtle.SetPenWeight(50);
            Turtle.Go(100);
            Game.Sleep(1000);
            Turtle.GoToCenter();
        }

        private static void Program12()
        {
            Game.ShowSceneAndAddTurtle();

            Turtle.OnKeyDown(Models.KeyboardKey.Down, player => player.ChangeSizeFactor(-0.1));
            Turtle.OnKeyDown(Models.KeyboardKey.Up, player => player.ChangeSizeFactor(0.1));
        }

        private static void Program13()
        {
            Game.ShowScene();

            var leftPlayer = Game.AddPlayer(
                Models.Player.Create(
                    Costumes.CreateRectangle(
                        new Models.Size(20, 150),
                        RGBColor.DarkMagenta)));
            leftPlayer.GoTo(Game.State.SceneBounds.Left + 20, 0);

            var rightPlayer = Game.AddPlayer(
                Models.Player.Create(
                    Costumes.CreateRectangle(
                        new Models.Size(20, 150),
                        RGBColor.Magenta)));
            rightPlayer.GoTo(Game.State.SceneBounds.Right - 20, 0);

            var ball = Game.AddPlayer(Models.Player.Create(Costumes.CreateCircle(10, RGBColor.Black)));
        }
    }
}
