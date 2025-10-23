using static pathfinding.Program;

namespace pathfinding
{
    internal class MapGeneration
    {
        public static Tile[,] Generate(int width, int height)
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

            /*
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        Map[x, y] = WallTile;
                    }
                }
            }
            */

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
