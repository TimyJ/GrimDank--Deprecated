using System;
using GrimDank.TerrainTypes;
using SadConsole.Surfaces;
using Microsoft.Xna.Framework;
using XnaRect = Microsoft.Xna.Framework.Rectangle;
using XnaInput = Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Input;
using GoRogue;

namespace GrimDank.Consoles
{
    // TODO: Sort out how to handle cell stuff arbitrarily (from multiple renderers)
    // This MAYBE should be an IScreen but input handling woudl be different and this is slightly less code
    // TODO: This probably needs to be IDisposable if MapRenderers are destroyed before map (lest lambda keep them alive)
    // Since these require private Map class data to function, you likely won't create these
    // using a constructor.  Instead, use the Map.CreateRenderer function.
    class MapConsole : SadConsole.Console
    {
        public Map CurrentMap { get; private set; }

        private Point cachedOffset;

        public MapConsole(int width, int height, Map map, Terrain[] terrain)
            : base(new BasicSurface(map.Width, map.Height, terrain, Global.FontDefault,
                                    new XnaRect(0, 0, width, height)))
        {
            UseKeyboard = true;

            CurrentMap = map;
            map.CellsDirtied += OnCellsDirtied;
        }

        // If render area has moved, we need to update all GameObject positions.
        public override void Update(TimeSpan timeElapsed)
        {
            base.Update(timeElapsed);

            UpdateGameObjectOffsets();
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            Direction toMoveIn = Direction.NONE;

            if (info.IsKeyPressed(XnaInput.Keys.NumPad8))
                toMoveIn = Direction.UP;
            else if (info.IsKeyPressed(XnaInput.Keys.NumPad9))
                toMoveIn = Direction.UP_RIGHT;
            else if (info.IsKeyPressed(XnaInput.Keys.NumPad6))
                toMoveIn = Direction.RIGHT;
            else if (info.IsKeyPressed(XnaInput.Keys.NumPad3))
                toMoveIn = Direction.DOWN_RIGHT;
            else if (info.IsKeyPressed(XnaInput.Keys.NumPad2))
                toMoveIn = Direction.DOWN;
            else if (info.IsKeyPressed(XnaInput.Keys.NumPad1))
                toMoveIn = Direction.DOWN_LEFT;
            else if (info.IsKeyPressed(XnaInput.Keys.NumPad4))
                toMoveIn = Direction.LEFT;
            else if (info.IsKeyPressed(XnaInput.Keys.NumPad7))
                toMoveIn = Direction.UP_LEFT;
            else
                return false; // We don't care about this input, let some other console on the stack take a look

            // Some input was used above (we didn't return false), so we should move.
            GrimDank.Player.MoveIn(toMoveIn);

            return true; // My input!  MINE!

        }

        // If the screen position has moved, we need to update all GameObject positions.
        public override void OnCalculateRenderPosition()
        {
            base.OnCalculateRenderPosition();

            UpdateGameObjectOffsets();
        }

        public override void Draw(TimeSpan timeElapsed)
        {
            base.Draw(timeElapsed);

            if (IsVisible)
            {
                // Draw GameObjects
                foreach (var gameObject in CurrentMap.Entities.Items)
                    gameObject.Renderer.Draw(timeElapsed);

            }
        }

        private void OnCellsDirtied(object s, EventArgs e) => textSurface.IsDirty = true;

        private void UpdateGameObjectOffsets()
        {
            var offset = position + textSurface.RenderArea.Location;
            if (offset != cachedOffset)
            {
                cachedOffset = offset;

                foreach (var gameObject in CurrentMap.Entities.Items)
                    gameObject.Renderer.PositionOffset = cachedOffset;
            }
        }
    }
}
