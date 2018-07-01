using Microsoft.Xna.Framework;
using GoRogue.MapGeneration.Generators;
using GoRogue.MapViews;
using GrimDank.TerrainTypes;
using GrimDank.MapObjects;
using GoRogue;


namespace GrimDank
{
    class GrimDank
    {
        public const int ScreenWidth = 80;
        public const int ScreenHeight = 25;

        // This is mad temp.
        public const int MapWidth = 100;
        public const int MapHeight = 100;

        public static Map CurrentMap { get; private set; }
        public static Player Player { get; private set; }

        static void Main(string[] args)
        {
            // Setup the engine and creat the main window.
            SadConsole.Game.Create("IBM.font", ScreenWidth, ScreenHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //

            SadConsole.Game.Instance.Dispose();
        }

        private static void Update(GameTime time)
        {
            // Called each logic update.

            // As an example, we'll use the F5 key to make the game full screen
            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                SadConsole.Settings.ToggleFullScreen();
            }
        }

        private static void Init()
        {
            CurrentMap = GenerateMap();
            AddPlayer(Coord.Get(5, 6));

            var mapScreen = CurrentMap.CreateRenderer(ScreenWidth, ScreenHeight);

            // Set our new console as the thing to render and process
            SadConsole.Global.CurrentScreen = mapScreen;

            SadConsole.Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.Stretch;
        }

        // Uhh... this no go here man... Yes this is temp, don't worry.
        private static Map GenerateMap()
        {
            var gennedMap = new Map(MapWidth, MapHeight);

            var walkabilityMap = new ArrayMap<bool>(gennedMap.Width, gennedMap.Height);
            RectangleMapGenerator.Generate(walkabilityMap);

            for (int x = 0; x < walkabilityMap.Width; x++)
                for (int y = 0; y < walkabilityMap.Height; y++)
                    if (walkabilityMap[x, y])
                        gennedMap.SetTerrain(x, y, new Floor(x, y));
                    else
                        gennedMap.SetTerrain(x, y, new Wall(x, y));

            return gennedMap;
        }

        // See comment on above function.
        private static void AddPlayer(Coord position)
        {
            Player = new Player(position);
            CurrentMap.Add(Player);
        }
    }
}
