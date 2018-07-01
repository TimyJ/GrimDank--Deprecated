using SadConsole.GameHelpers;
using Microsoft.Xna.Framework;
using GoRogue;

namespace GrimDank.MapObjects
{
    class MapObject : IHasID
    {
        private static readonly IDGenerator generator = new IDGenerator();

        public uint ID { get; private set; }

        public GameObject Renderer { get; private set; }

        public MapObject(Color foreground, Color background, int glyph)
        {
            ID = generator.UseID();

            Renderer = new GameObject(1, 1);

            Renderer.Animation.CurrentFrame[0].Glyph = glyph;
            Renderer.Animation.CurrentFrame[0].Foreground = foreground;
            Renderer.Animation.CurrentFrame[0].Background = background;
        }
    }
}
