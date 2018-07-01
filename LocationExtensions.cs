using GoRogue;
using Microsoft.Xna.Framework;

namespace GrimDank
{
    // Convenience extension functions for working with SadConsole points and GoRogue stuff.
    static class LocationExtensions
    {
        public static Coord ToCoord(this Point p) => Coord.Get(p.X, p.Y);
        public static Point ToPoint(this Coord c) => new Point(c.X, c.Y);
        public static Coord Add(this Coord c, Point p) => Coord.Get(c.X + p.X, c.Y + p.Y);
        public static Point Add(this Point p, Coord c) => new Point(p.X + c.X, p.Y + c.Y);
        public static Point Add(this Point p, Direction d) => new Point(p.X + d.DeltaX, p.Y + d.DeltaY);
    }
}
