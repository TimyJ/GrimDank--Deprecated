using GrimDank.TerrainTypes;
using GrimDank.MapObjects;
using GoRogue;
using System;

namespace GrimDank
{
    class MapObjectArgs : EventArgs
    {
        public MapObject MapObject { get; private set; }

        public MapObjectArgs(MapObject mapObject)
        {
            MapObject = mapObject;
        }
    }

    class Map
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private Terrain[] terrain;

        // We'll need multiple spatial maps (one per layer probably) later on.  But,
        // this is a good starting point.
        private SpatialMap<MapObject> _entities;
        public IReadOnlySpatialMap<MapObject> Entities { get => _entities.AsReadOnly(); }

        // Occurs when a cell has been changed and thus renderers need to re-compute textures, provided
        // that either the TerrainBase property setters were used, or the MarkCellsDirty function is called.
        public event EventHandler CellsDirtied;

        public event EventHandler<MapObjectArgs> MapObjectAdded;
        public event EventHandler<MapObjectArgs> MapObjectRemoved;

        public Map(int width, int height)
        {
            Width = width;
            Height = height;

            terrain = new Terrain[Width * Height];
            _entities = new SpatialMap<MapObject>();

            CellsDirtied = null;
            MapObjectAdded = null;
            MapObjectRemoved = null;
        }

        public Terrain GetTerrain(Coord position) => terrain[position.ToIndex(Width)];
        public Terrain GetTerrain(int x, int y) => terrain[Coord.ToIndex(x, y, Width)];

        public void SetTerrain(Coord position, Terrain newTerrain)
        {
            int index = position.ToIndex(Width);
            if (terrain[index] != null)
                terrain[index].CellChanged -= OnCellChanged;

            terrain[index] = newTerrain;

            if (terrain[index] != null)
                terrain[index].CellChanged += OnCellChanged;

            MarkCellsDirty();
        }
        public void SetTerrain(int x, int y, Terrain newTerrain) => SetTerrain(Coord.Get(x, y), newTerrain);

        // Call this when you are changing something about a piece of terrain, and you're doing it in a way where
        // the setter functions for Terrain won't be called (eg., while that terrain is casted to a Cell, or while
        // using a SadConsole function that manipulates its properties with it casted as a Cell.
        public void MarkCellsDirty() => CellsDirtied?.Invoke(this, EventArgs.Empty);

        // Create renderer for this map.
        public MapRenderer CreateRenderer(int width, int height) =>
            new MapRenderer(width, height, this, terrain);

        // Event handler for TerrainBase.CellChanged event.
        private void OnCellChanged(object s, EventArgs e) => MarkCellsDirty();
    }
}
