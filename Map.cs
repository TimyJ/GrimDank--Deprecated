using GrimDank.TerrainTypes;
using GrimDank.MapObjects;
using GrimDank.Consoles;
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

        public event EventHandler<MovedArgs> MapObjectMoved;

        public Map(int width, int height)
        {
            Width = width;
            Height = height;

            terrain = new Terrain[Width * Height];
            _entities = new SpatialMap<MapObject>();

            CellsDirtied = null;
            MapObjectAdded = null;
            MapObjectRemoved = null;
            MapObjectMoved = null;
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

        public bool Add(MapObject mapObject) => Add(mapObject, mapObject.Position);

        // Adds object at given position by first modifying the position and then adding the object
        public bool Add(MapObject mapObject, Coord position)
        {
            if (WillCollide(mapObject, position))
                return false;

            // TODO: This will change when we switch to layers.
            if (_entities.Contains(position))
                return false;

            if (mapObject.CurrentMap != null)
                mapObject.CurrentMap.Remove(mapObject);

            mapObject.Position = position;

            _entities.Add(mapObject, mapObject.Position);
            mapObject.Moved += OnMapObjectMoved;
            mapObject._onMapChanged(this);

            MapObjectAdded?.Invoke(this, new MapObjectArgs(mapObject));

            return true;
        }
        public bool Add(MapObject mapObject, int x, int y) => Add(mapObject, Coord.Get(x, y));

        public bool Remove(MapObject mapObject)
        {
            // TODO: This will change when we switch to layers.
            if (!_entities.Remove(mapObject))
                return false;

            mapObject.Moved -= OnMapObjectMoved;
            mapObject._onMapChanged(null);

            MapObjectRemoved?.Invoke(this, new MapObjectArgs(mapObject));

            return true;
        }

        // Call this when you are changing something about a piece of terrain, and you're doing it in a way where
        // the setter functions for Terrain won't be called (eg., while that terrain is casted to a Cell, or while
        // using a SadConsole function that manipulates its properties with it casted as a Cell.
        public void MarkCellsDirty() => CellsDirtied?.Invoke(this, EventArgs.Empty);

        public bool IsWalkable(Coord position)
        {
            if (!GetTerrain(position).IsWalkable)
                return false;

            // TODO: These will change once we add multiple spatial maps.
            if (!_entities.Contains(position))
                return true;

            if (!_entities.GetItem(position).IsWalkable)
                return false;

            return true;

        }
        public bool IsWalkable(int x, int y) => IsWalkable(Coord.Get(x, y));

        // Whether or not the given item will collide at the given position
        public bool WillCollide(IMapThing item, Coord position)
        {
            if (item.IsWalkable)
                return false;

            if (IsWalkable(position))
                return false;

            return true;
        }
        public bool WillCollide(IMapThing item, int x, int y) => WillCollide(item, Coord.Get(x, y));
        // Assumes current position of item as the position to check
        public bool WillCollide(IMapThing item) => WillCollide(item, item.Position);

        // Create renderer for this map.
        public MapConsole CreateRenderer(int width, int height) =>
            new MapConsole(width, height, this, terrain);

        // Event handler for TerrainBase.CellChanged event.
        private void OnCellChanged(object s, EventArgs e) => MarkCellsDirty();

        // Assumes collision detection already done, just to keep spatial map up to date.
        private void OnMapObjectMoved(object s, MovedArgs e)
        {
            _entities.Move((MapObject)s, e.NewPosition);

            MapObjectMoved?.Invoke(s, e);
        }
    }
}
