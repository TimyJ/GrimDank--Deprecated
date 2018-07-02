using GoRogue;
using Microsoft.Xna.Framework;

namespace GrimDank.TerrainTypes
{
    class Floor : Terrain
    {
        public Floor(Coord position)
            : base(position, '.', Color.White, Color.DarkGreen, true, true) { }

        public Floor(int x, int y)
            : this(Coord.Get(x, y)) { }
    }
}
