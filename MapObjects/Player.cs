using GoRogue;
using Microsoft.Xna.Framework;

namespace GrimDank.MapObjects
{
    class Player : MapObject
    {
        public Player(Coord position)
            : base(position, '@', Color.White, Color.Transparent, false, true)
        {
            // This could likely be handled better from a camera class but this works for the moment
            Moved += OnPlayerMoved;
        }

        public Player(int x, int y) : this(Coord.Get(x, y)) { }

        private void OnPlayerMoved(object s, MovedArgs e)
        {
            if (GrimDank.MapConsole != null && CurrentMap == GrimDank.MapConsole.CurrentMap)
                GrimDank.MapConsole.CenterViewportOn(Position);
        }
    }
}
