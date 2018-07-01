using System;
using System.Collections.Generic;
using GrimDank.TerrainTypes;
using SadConsole.Renderers;
using SadConsole.Surfaces;
using Microsoft.Xna.Framework;
using SadConsole;

namespace GrimDank
{
    // TODO: This probably needs to be IDisposable if MapRenderers are destroyed before map (lest lambda keep them alive)
    // Since these require private Map class data to function, you likely won't create these
    // using a constructor.  Instead, use the Map.CreateRenderer function.
    class MapScreen : Screen
    {
        public Map CurrentMap { get; private set; }
        public bool UsePixelPositioning { get; set; }

        private ISurface textSurface;
        private ISurfaceRenderer surfaceRenderer;

        private Point cachedOffset;

        public MapScreen(int width, int height, Map map, Terrain[] terrain)
        {
            UsePixelPositioning = false;

            CurrentMap = map;

            // TODO: This should be min of this and map width/height, probably.
            var viewport = new Rectangle(0, 0, width, height);
            textSurface = new BasicSurface(map.Width, map.Height, terrain, Global.FontDefault, viewport);
            surfaceRenderer = new SurfaceRenderer();

            map.CellsDirtied += OnCellsDirtied;
        }

        // If render area has moved, we need to update all GameObject positions.
        public override void Update(TimeSpan timeElapsed)
        {
            base.Update(timeElapsed);

            var offset = position + textSurface.RenderArea.Location;
            if (offset != cachedOffset)
            {
                cachedOffset = offset;

                foreach (var gameObject in CurrentMap.Entities.Items)
                    gameObject.Renderer.PositionOffset = cachedOffset;
            }
        }

        // If the screen position has moved, we need to update all GameObject positions.
        public override void OnCalculateRenderPosition()
        {
            base.OnCalculateRenderPosition();

            var offset = position + textSurface.RenderArea.Location;
            if (offset != cachedOffset)
            {
                cachedOffset = offset;

                foreach (var gameObject in CurrentMap.Entities.Items)
                    gameObject.Renderer.PositionOffset = cachedOffset;
            }
        }

        public override void Draw(TimeSpan timeElapsed)
        {
            if (IsVisible)
            {
                // Draw the cells (terrain)
                surfaceRenderer.Render(textSurface);
                Global.DrawCalls.Add(new DrawCallSurface(textSurface, calculatedPosition, UsePixelPositioning));

                // Draw GameObjects
                foreach (var gameObject in CurrentMap.Entities.Items)
                    gameObject.Renderer.Draw(timeElapsed);

                // Draw any children screens.
                if (Children.Count != 0)
                {
                    var copyList = new List<IScreen>(Children);

                    foreach (var child in copyList)
                        child.Draw(timeElapsed);
                }
            }
        }

        private void OnCellsDirtied(object s, EventArgs e) => textSurface.IsDirty = true;
    }
}
