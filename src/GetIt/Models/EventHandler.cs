using System;
using OneOf;

namespace GetIt.Models
{

    public abstract class EventHandler
        : OneOfBase<
            EventHandler.KeyDown,
            EventHandler.ClickPlayer,
            EventHandler.MouseEnterPlayer>
    {
        public abstract void Handle(Event ev);

        [Equals]
        public sealed class KeyDown : EventHandler
        {
            private readonly KeyboardKey key;
            private readonly Action handler;

            public KeyDown(KeyboardKey key, Action handler)
            {
                this.key = key;
                this.handler = handler;
            }

            public Guid Id { get; } = Guid.NewGuid();

            public override void Handle(Event ev)
            {
                if (ev is Event.KeyDown e && e.Key == key)
                {
                    handler();
                }
            }
        }

        [Equals]
        public sealed class ClickScene : EventHandler
        {
            private readonly Action<Position> handler;

            public ClickScene(Action<Position> handler)
            {
                this.handler = handler;
            }

            public Guid Id { get; } = Guid.NewGuid();

            public override void Handle(Event ev)
            {
                if (ev is Event.ClickScene e)
                {
                    handler(e.Position);
                }
            }
        }

        [Equals]
        public sealed class ClickPlayer : EventHandler
        {
            private readonly Guid playerId;
            private readonly Action handler;

            public ClickPlayer(Guid playerId, Action handler)
            {
                this.playerId = playerId;
                this.handler = handler;
            }

            public Guid Id { get; } = Guid.NewGuid();

            public override void Handle(Event ev)
            {
                if (ev is Event.ClickPlayer e && e.PlayerId == playerId)
                {
                    handler();
                }
            }
        }

        [Equals]
        public sealed class MouseEnterPlayer : EventHandler
        {
            private readonly Guid playerId;
            private readonly Action handler;

            public MouseEnterPlayer(Guid playerId, Action handler)
            {
                this.playerId = playerId;
                this.handler = handler;
            }

            public Guid Id { get; } = Guid.NewGuid();

            public override void Handle(Event ev)
            {
                if (ev is Event.MouseEnterPlayer e && e.PlayerId == playerId)
                {
                    handler();
                }
            }
        }
    }
}