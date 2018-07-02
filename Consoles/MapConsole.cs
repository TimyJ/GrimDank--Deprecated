using System;
using GrimDank.TerrainTypes;
using GrimDank.MapObjects;
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

        public int ViewportWidth { get; private set; }
        public int ViewportHeight { get; private set; }

        public MapConsole(int width, int height, Map map, Terrain[] terrain)
            : base(new BasicSurface(map.Width, map.Height, terrain, Global.FontDefault,
                                    new XnaRect(0, 0, width, height)))
        {
            ViewportWidth = width;
            ViewportHeight = height;

            UseKeyboard = true;

            CurrentMap = map;
            map.CellsDirtied += OnCellsDirtied;
            map.MapObjectAdded += OnMapObjectAdded;
            map.MapObjectMoved += OnMapObjectMoved;

            UpdateGameObjectOffsets(true);
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

        // Either we can toggle visibility for stuff as we go, or we can jack the cell rendering and only add those that we need to render for FOV.  Either way.
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

        public void CenterViewportOn(Coord position)
        {
            textSurface.RenderArea = new XnaRect(position.X - (ViewportWidth / 2), position.Y - (ViewportHeight / 2), ViewportWidth, ViewportHeight);
        }

        private void OnCellsDirtied(object s, EventArgs e) => textSurface.IsDirty = true;

        private void OnMapObjectAdded(object s, EventArgs e) => UpdateGameObjectOffset((MapObject)s);

        private void OnMapObjectMoved(object s, MovedArgs e) => UpdateGameObjectOffset((MapObject)s);

        private void UpdateGameObjectOffsets(bool force = false)
        {
            //var offset = position + textSurface.RenderArea.Location;
            var offset = CalculatedPosition - textSurface.RenderArea.Location;
            if (offset != cachedOffset || force)
            {
                cachedOffset = offset;

                foreach (var mapObject in CurrentMap.Entities.Items)
                    UpdateGameObjectOffset(mapObject);
            }
        }

        private void UpdateGameObjectOffset(MapObject mapObject)
        {
            mapObject.Renderer.PositionOffset = cachedOffset;
            mapObject.Renderer.IsVisible = textSurface.RenderArea.Contains(mapObject.Renderer.Position);
        }
    }
}
