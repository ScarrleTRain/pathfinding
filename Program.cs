using System.Text;
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
            public bool IsDiscovered;
            public bool IsInView;

            public Tile(char displayChar, bool isWalkable, ConsoleColor color)
            {
                DisplayChar = displayChar;
                IsWalkable = isWalkable;
                Color = color;
                IsDiscovered = false;
            }
        }

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

            Map = MapGeneration.Generate(Console.WindowWidth, Console.WindowHeight);

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
            FOV.ComputeFOV(Character.X, Character.Y, 5);

            while (true)
            {
                Render();

                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.DownArrow && Character.Y < Map.GetLength(1) && Map[Character.X, Character.Y + 1].IsWalkable)
                {
                    Character.Y++;
                }
                else if (key.Key == ConsoleKey.UpArrow && Character.Y > 0 && Map[Character.X, Character.Y - 1].IsWalkable)
                {
                    Character.Y--;
                }
                else if (key.Key == ConsoleKey.LeftArrow && Character.X > 0 && Map[Character.X - 1, Character.Y].IsWalkable)
                {
                    Character.X--;
                }
                else if (key.Key == ConsoleKey.RightArrow && Character.X < Map.GetLength(0) && Map[Character.X + 1, Character.Y].IsWalkable)
                {
                    Character.X++;
                }

                FOV.ComputeFOV(Character.X, Character.Y, 5);
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
                        targetTile.Color != MapBuffer[x, y].Color ||
                        targetTile.IsDiscovered != MapBuffer[x, y].IsDiscovered ||
                        targetTile.IsInView != MapBuffer[x, y].IsInView)
                    {
                        // They are different! Draw the new tile.
                        Console.SetCursorPosition(x, y);

                        if (targetTile.IsInView)
                        {
                            // In view: Draw bright and clear
                            // (This is the only check you need to add)
                            Console.ForegroundColor = targetTile.Color;
                            Console.Write(targetTile.DisplayChar);
                        }
                        else if (targetTile.IsDiscovered)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray; // Dim it
                            Console.Write(targetTile.DisplayChar);
                        }
                        else
                        {
                            // Not discovered: Draw empty space
                            Console.Write(' ');
                        }

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
            Console.OutputEncoding = Encoding.UTF8;
            Random rand = new();
            StringBuilder sb = new(capacity: 5);

            while (!token.IsCancellationRequested)
            {
                sb.Clear();
                for (int i = 0; i < 10; i++)
                    sb.Append((char)(0x2800 + rand.Next(0x100)));

                Console.Write($"\r{sb}");
                Thread.Sleep(100);
            }

            Console.OutputEncoding = Encoding.Default;
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
    }
}
