using GoRogue;

namespace GrimDank
{
    // Just defines a common interface for MapObject and Terrain, so some generic collision checking functions
    // can be written
    interface IMapThing
    {
        bool IsWalkable { get; }
        bool IsTransparent { get; }

        Coord Position { get; }
    }
}
