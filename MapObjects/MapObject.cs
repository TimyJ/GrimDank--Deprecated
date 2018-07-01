using SadConsole.GameHelpers;
using Microsoft.Xna.Framework;
using GoRogue;
using System;

namespace GrimDank.MapObjects
{
    class MovedArgs : EventArgs
    {
        public Coord OldPosition { get; private set; }
        public Coord NewPosition { get; private set; }

        public MovedArgs(Coord oldPosition, Coord newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
    }

    class MapObject : IHasID, IMapThing
    {
        private static readonly IDGenerator generator = new IDGenerator();

        public uint ID { get; private set; }

        public GameObject Renderer { get; private set; }

        public bool IsWalkable { get; private set; }

        public bool IsTransparent { get; private set; }

        public Coord Position
        {
            get => Renderer.Position.ToCoord();
            set
            {
                if (value != Position)
                {
                    var oldPos = Position;
                    if (CurrentMap == null || !CurrentMap.WillCollide(this, value))
                    {
                        Renderer.Position = value.ToPoint();
                        Moved?.Invoke(this, new MovedArgs(oldPos, value));
                    }
                }
            }
          
        }

        public Map CurrentMap { get; private set; }

        public event EventHandler<MovedArgs> Moved;

        public MapObject(Coord position, int glyph, Color foreground, Color background,
                          bool isWalkable = false, bool isTransparent = true)
        {
            ID = generator.UseID();

            Renderer = new GameObject(1, 1);
            Position = position;

            Renderer.Animation.CurrentFrame[0].Glyph = glyph;
            Renderer.Animation.CurrentFrame[0].Foreground = foreground;
            Renderer.Animation.CurrentFrame[0].Background = background;

            IsWalkable = isWalkable;
            IsTransparent = isTransparent;

            Moved = null;
        }

        public MapObject(int x, int y, int glyph, Color foreground, Color background,
                          bool isWalkable = false, bool isTransparent = true)
            : this(Coord.Get(x, y), glyph, foreground, background, isWalkable, isTransparent) { }

        public bool MoveIn(Direction direction)
        {
            if (direction == Direction.NONE)
                return false;

            var oldPos = Position;
            Position += direction;

            return Position != oldPos;
        }

        // Do NOT call this unless you are the map class's Add/Remove GameObject functions.  Bad.
        internal void _onMapChanged(Map newMap) => CurrentMap = newMap;
    }
}
