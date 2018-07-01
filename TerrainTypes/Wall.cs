using GoRogue;
using Microsoft.Xna.Framework;

namespace GrimDank.TerrainTypes
{
    class Wall : Terrain
    {
        public Wall(Coord position)
            : base(position, '#', Color.White, Color.Transparent, false, false) { }

        public Wall(int x, int y)
            : this(Coord.Get(x, y)) { }
    }
}
