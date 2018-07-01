using SadConsole;
using Microsoft.Xna.Framework;
using System;
using GoRogue;

namespace GrimDank.TerrainTypes
{
    class Terrain : Cell, IMapThing
    {
        // We hide these with new.  It's like inheritance but without polymorphism.  If I cast this to Cell
        // and modify Foreground for instance, the below getter/setter aren't called -- Cell's getter and setter
        // are.  Because this is necessary to make sure Renderers know when their cells change, we need to be
        // careful not to call SadConsole functions that change a Cell without calling the Map's MarkCellsDirty function
        public new Color Foreground
        {
            get => base.Foreground;
            set
            {
                if (base.Foreground != value)
                {
                    base.Foreground = value;
                    CellChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public new Color Background
        {
            get => base.Background;
            set
            {
                if (base.Background != value)
                {
                    base.Foreground = value;
                    CellChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public new int Glyph
        {
            get => base.Glyph;
            set
            {
                if (base.Glyph != value)
                {
                    base.Glyph = value;
                    CellChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        // Walls don't move, yo.
        public Coord Position { get; private set; }

        public bool IsWalkable { get; set; }
        public bool IsTransparent { get; set; }

        public event EventHandler CellChanged;

        public Terrain(int x, int y, int glyph, Color foreground, Color background,
                        bool isWalkable = true, bool isTransparent = true)
            : this(Coord.Get(x, y), glyph, foreground, background, isWalkable, isTransparent) { }

        public Terrain(Coord position, int glyph, Color foreground, Color background,
                        bool isWalkable = true, bool isTransparent = true)
            : base(foreground, background, glyph)
        {
            Position = position;

            IsWalkable = isWalkable;
            IsTransparent = isTransparent;
        }
    }
}
