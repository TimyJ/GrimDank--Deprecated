using System;
using SadConsole.GameHelpers;
using SadConsole;
using SadConsole.Surfaces;

namespace GrimDank
{
    /// <summary>
    /// Game object that, when parented to a console, will render if and only if
    /// it is within that console's surface view rectangle, and renders in the
    /// appropriate position.  Effectively a game object that manages its own
    /// PositionOffset.
    /// </summary>
    public class WorldBasedGameObject : GameObject
    {
        public WorldBasedGameObject(int width, int height) : base(width, height) { }
        public WorldBasedGameObject(int width, int height, Font font) : base(width, height, font) { }
        public WorldBasedGameObject(AnimatedSurface animation) : base(animation) { }

        // TODO: Better would be custom console that has event for render and calc pos changed, subscribe on parented.
        public override void Draw(TimeSpan timeElapsed)
        {
            if (IsVisible)
            {
                var console = Parent as IConsole;
                if (console != null && PositionOffset != console.CalculatedPosition - console.TextSurface.RenderArea.Location)
                    PositionOffset = console.CalculatedPosition - console.TextSurface.RenderArea.Location;

                if (console.TextSurface.RenderArea.Contains(Position))
                {
                    renderer.Render(animation);
                    Global.DrawCalls.Add(new DrawCallSurface(animation, calculatedPosition - animation.Center, usePixelPositioning));
                    base.Draw(timeElapsed);
                }
            }
        }
    }
}
