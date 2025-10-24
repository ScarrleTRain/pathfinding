using static pathfinding.Program;

namespace pathfinding
{
    internal class FOV
    {
        public static void ComputeFOV(int playerX, int playerY, int radius)
        {
            for (int y = 0; y < Map.GetLength(1); y++)
            {
                for (int x = 0; x < Map.GetLength(0); x++)
                {
                    Tile tile = Map[x, y];
                    tile.IsInView = false;
                    Map[x, y] = tile;
                }
            }

            int startX = Math.Max(0, playerX - radius);
            int endX = Math.Min(Map.GetLength(0) - 1, playerX + radius);
            int startY = Math.Max(0, playerY - radius);
            int endY = Math.Min(Map.GetLength(1) - 1, playerY + radius);

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    if ((x - playerX) * (x - playerX) + (y - playerY) * (y - playerY) > radius * radius)
                    {
                        continue;
                    }

                    if (HasLineOfSight(playerX, playerY, x, y))
                    {
                        Tile tile = Map[x, y];
                        tile.IsInView = true;
                        tile.IsDiscovered = true;
                        Map[x, y] = tile;
                    }
                }
            }
        }

        private static bool HasLineOfSight(int x1, int y1, int x2, int y2)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if (!Map[x1, y1].IsWalkable)
                {
                    return (x1 == x2 && y1 == y2);
                }

                if (x1 == x2 && y1 == y2)
                {
                    return true;
                }

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y1 += sy;
                }
            }
        }
    }
}
