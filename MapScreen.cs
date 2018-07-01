using System;
using GrimDank.TerrainTypes;
using SadConsole.Surfaces;
using Microsoft.Xna.Framework;
using SadConsole;

namespace GrimDank
{
    // This MAYBE should be an IScreen but input handling woudl be different and this is slightly less code
    // TODO: This probably needs to be IDisposable if MapRenderers are destroyed before map (lest lambda keep them alive)
    // Since these require private Map class data to function, you likely won't create these
    // using a constructor.  Instead, use the Map.CreateRenderer function.
    class MapScreen : SadConsole.Console
    {
        public Map CurrentMap { get; private set; }

        private Point cachedOffset;

        public MapScreen(int width, int height, Map map, Terrain[] terrain)
            : base(new BasicSurface(map.Width, map.Height, terrain, Global.FontDefault,
                                    new Rectangle(0, 0, width, height)))
        {
            CurrentMap = map;
            map.CellsDirtied += OnCellsDirtied;
        }

        // If render area has moved, we need to update all GameObject positions.
        public override void Update(TimeSpan timeElapsed)
        {
            base.Update(timeElapsed);

            UpdateGameObjectOffsets();
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
