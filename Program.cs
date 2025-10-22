using static pathfinding.Program;

namespace pathfinding
{
    internal class Program
    {
        public struct Tile
        {
            public char DisplayChar;
            public bool IsWalkable;
            public ConsoleColor Color;

            public Tile(char displayChar, bool isWalkable, ConsoleColor color)
            {
                DisplayChar = displayChar;
                IsWalkable = isWalkable;
                Color = color;
            }
        }

        public static readonly Tile WallTile = new Tile('#', false, ConsoleColor.Gray);
        public static readonly Tile FloorTile = new Tile('·', true, ConsoleColor.DarkGray);

        public class Entity
        {
            public int X { get; set; }
            public int Y { get; set; }
            public char DisplayChar { get; set; }
            public ConsoleColor Color { get; set; }
        }

        public class Player : Entity
        {
            public int Health { get; set; }
            // Other stats idk
        }

        public class Enemy : Entity
        {
            public bool AI { get; set; }
        }

        public static Player Character = new Player() { DisplayChar = '@', Color = ConsoleColor.Cyan };
        public static Tile[,] Map;
        public static Tile[,] MapBuffer;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            using var cts = new CancellationTokenSource();
            var spinnerTask = Task.Run(() => Throb(cts.Token));

            Map = GenerateCaves(Console.WindowWidth, Console.WindowHeight);

            cts.Cancel();
            spinnerTask.Wait();
            Console.Clear();

            for (int x = 0; x < Map.GetLength(0); x++)
                for (int y = 0; y < Map.GetLength(1); y++)
                    if (Map[x, y].IsWalkable)
                    {
                        Character.X = x;
                        Character.Y = y;
                    }

            ConsoleKeyInfo key;

            while (true)
            {
                Render();
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.DownArrow && Map[Character.X, Character.Y + 1].IsWalkable)
                {
                    Character.Y++;
                }
                else if (key.Key == ConsoleKey.UpArrow && Map[Character.X, Character.Y - 1].IsWalkable)
                {
                    Character.Y--;
                }
                else if (key.Key == ConsoleKey.LeftArrow && Map[Character.X - 1, Character.Y].IsWalkable)
                {
                    Character.X--;
                }
                else if (key.Key == ConsoleKey.RightArrow && Map[Character.X + 1, Character.Y].IsWalkable)
                {
                    Character.X++;
                }
            }
        }

        public static void Render()
        {
            int width = Map.GetLength(0);
            int height = Map.GetLength(1);

            // 1. Draw the map tiles
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get the tile that *should* be here
                    Tile targetTile = Map[x, y];

                    // Check it against the buffer
                    if (targetTile.DisplayChar != MapBuffer[x, y].DisplayChar ||
                        targetTile.Color != MapBuffer[x, y].Color)
                    {
                        // They are different! Draw the new tile.
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = targetTile.Color;
                        Console.Write(targetTile.DisplayChar);

                        // Update the buffer
                        MapBuffer[x, y] = targetTile;
                    }
                }
            }

            Tile playerTile = new Tile(Character.DisplayChar, false, Character.Color);
            if (playerTile.DisplayChar != MapBuffer[Character.X, Character.Y].DisplayChar ||
                playerTile.Color != MapBuffer[Character.X, Character.Y].Color)
            {
                Console.SetCursorPosition(Character.X, Character.Y);
                Console.ForegroundColor = playerTile.Color;
                Console.Write(playerTile.DisplayChar);

                MapBuffer[Character.X, Character.Y] = playerTile;
            }
        }

        public static void Throb(CancellationToken token)
        {
            var spinner = new[] { '|', '/', '-', '\\' };
            int i = 0;
            while (!token.IsCancellationRequested)
            {
                Console.Write($"\r{spinner[i++ % spinner.Length]}");
                Thread.Sleep(100);
            }

            Console.Write("\r ");
        }

        public static void DrawSimple()
        {
            Console.SetCursorPosition(0, 0);

            for (int y = 0; y < Map.GetLength(1); y++)
            {
                for (int x = 0; x < Map.GetLength(0); x++)
                {
                    Tile tile = Map[x, y];
                    Console.ForegroundColor = tile.Color;
                    Console.Write(tile.DisplayChar);
                }
                if (y != Map.GetLength(1) - 1) Console.WriteLine();
            }
        }

        public static Tile[,] GenerateCaves(int width, int height)
        {
            Map = new Tile[width, height];
            MapBuffer = new Tile[width, height];
            Random rand = new Random();

            Tile empty = new Tile(' ', false, ConsoleColor.Black);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    MapBuffer[x, y] = empty;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        Map[x, y] = WallTile;
                    }
                    else
                    {
                        Map[x, y] = rand.Next(0, 100) < 45 ? WallTile : FloorTile;
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                Tile[,] newMap = (Tile[,])Map.Clone();

                for (int x = 0; x < width - 1; x++)
                {
                    for (int y = 0; y < height - 1; y++)
                    {
                        int wallNeighbours = CountWallNeighbours(x, y);

                        if (!Map[x, y].IsWalkable)
                        {
                            if (wallNeighbours < 4) newMap[x, y] = FloorTile;
                        }
                        else
                        {
                            if (wallNeighbours > 4) newMap[x, y] = WallTile;
                        }
                    }
                }

                Map = newMap;
            }

            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
            //        {
            //            Map[x, y] = WallTile;
            //        }
            //    }
            //}

            return Map;
        }

        public static int CountWallNeighbours(int x, int y)
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    try
                    {
                        if (!Map[x + i, y + j].IsWalkable)
                        {
                            count++;
                        }
                    }
                    catch { }
                }
            }
            return count;
        }
    }
}
