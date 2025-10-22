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

        public static readonly Tile WallTile = new Tile('#', false, ConsoleColor.White);
        public static readonly Tile FloorTile = new Tile('·', true, ConsoleColor.Gray);

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

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            Tile[,] map = GenerateCaves(Console.WindowWidth, Console.WindowHeight);

            DrawSimple(map);

            while (true)
            {
            }
        }

        // (Assuming you have: Tile[,] map, Player player, List<Enemy> enemies)

        public static void DrawSimple(Tile[,] map)
        {
            // 1. Draw the map
            // We set the cursor once at the top-left to avoid flicker
            Console.SetCursorPosition(0, 0);

            for (int y = 0; y < map.GetLength(1); y++) // map.GetLength(1) is height
            {
                for (int x = 0; x < map.GetLength(0); x++) // map.GetLength(0) is width
                {
                    Tile tile = map[x, y];
                    Console.ForegroundColor = tile.Color;
                    Console.Write(tile.DisplayChar);
                }
                if (y != map.GetLength(1) - 1) Console.WriteLine();
            }
        }

        public static Tile[,] GenerateCaves(int width, int height)
        {
            Tile[,] map = new Tile[width, height];
            Random rand = new Random();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        map[x, y] = WallTile;
                    }
                    else
                    {
                        map[x, y] = rand.Next(0, 100) < 45 ? WallTile : FloorTile;
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                Tile[,] newMap = (Tile[,])map.Clone();

                for (int x = 0; x < width - 1; x++)
                {
                    for (int y = 0; y < height - 1; y++)
                    {
                        int wallNeighbours = CountWallNeighbours(map, x, y);

                        if (!map[x, y].IsWalkable)
                        {
                            if (wallNeighbours < 4) newMap[x, y] = FloorTile;
                        }
                        else
                        {
                            if (wallNeighbours > 4) newMap[x, y] = WallTile;
                        }
                    }
                }

                map = newMap;
            }

            return map;
        }

        public static int CountWallNeighbours(Tile[,] map, int x, int y)
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    try
                    {
                        if (!map[x + i, y + j].IsWalkable)
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
