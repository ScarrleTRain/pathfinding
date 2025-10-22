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

        public Tile WallTile = new Tile('#', false, ConsoleColor.White);
        public Tile FloorTile = new Tile('.', true, ConsoleColor.Gray);

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
            Console.WriteLine("Hello, World!");
        }

        public Tile[,] GenerateCaves(int width, int height)
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

            }
        }

        int CountWallNeighbours(Tile[,] map, int x, int y)
        {

        }
    }
}
