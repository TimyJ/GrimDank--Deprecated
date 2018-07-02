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

        public const int MapConsoleWidth = 70;
        public const int MapConsoleHeight = 20;

        // This is mad temp.
        public const int MapWidth = 100;
        public const int MapHeight = 100;

        public static Map CurrentMap { get; private set; }
        public static Player Player { get; private set; }

        // Set to true to enable FPS counter in top-left corner.
        public const bool ENABLE_FPS_COUNTER = false;

        static void Main(string[] args)
        {
            // Uncommenting this SHOULD allow unlimited FPS for performance testing, however this flag seems
            // broken at the moment, as it's still capped at 60.
            //SadConsole.Settings.UnlimitedFPS = true;

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
            var fpsCounter = new SadConsole.Game.FPSCounterComponent(SadConsole.Game.Instance);
            SadConsole.Game.Instance.Components.Add(fpsCounter);
            fpsCounter.Enabled = ENABLE_FPS_COUNTER;
            fpsCounter.Visible = ENABLE_FPS_COUNTER;

            CurrentMap = GenerateMap();
            AddPlayer(Coord.Get(5, 6));
            AddTestDummy(Coord.Get(73, 23));

            var mapScreen = CurrentMap.CreateRenderer(MapConsoleWidth, MapConsoleHeight);

            // Set our new console as the thing to render and process
            SadConsole.Global.CurrentScreen = mapScreen;
            SadConsole.Global.FocusedConsoles.Set(mapScreen);

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

        // See above comment.
        private static void AddTestDummy(Coord position)
        {
            CurrentMap.Add(new TestDummy(position));
        }
    }
}
