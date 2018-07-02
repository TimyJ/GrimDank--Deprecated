using GoRogue;
using Microsoft.Xna.Framework;

namespace GrimDank.MapObjects
{
    class Player : MapObject
    {
        public Player(Coord position)
            : base(position, '@', Color.White, Color.Transparent, false, true) { }

        public Player(int x, int y) : this(Coord.Get(x, y)) { }
    }
}
