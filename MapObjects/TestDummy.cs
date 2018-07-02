using Microsoft.Xna.Framework;
using GoRogue;

namespace GrimDank.MapObjects
{
    class TestDummy : MapObject
    {
        public TestDummy(Coord position)
            : base(position, 'g', Color.Red, Color.Transparent, false, true) { }

        public TestDummy(int x, int y) : this(Coord.Get(x, y)) { }
    }
}
