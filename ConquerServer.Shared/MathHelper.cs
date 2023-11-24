using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Shared;

namespace ConquerServer
{
    public static class MathHelper
    {
        public static double GetDistance2D(int x, int y, int nx, int ny)
        {
            double distance;
            double XHalf = Math.Pow((nx - x), 2);
            double YHalf = Math.Pow(ny - y, 2);
            distance = Math.Sqrt((XHalf + YHalf));
            return distance;
        }
        public static int GetDistance(int x, int y, int nx, int ny)
        {
            return Math.Max(Math.Abs(x-nx), Math.Abs(y-ny));
        }

        public static bool IsInAngleRange(int lower, int angle, int upper)
        {
            // https://stackoverflow.com/questions/71881043/how-to-check-angle-in-range
            lower %= 360;
            angle %= 360;
            upper %= 360;

            // normalize on the same circle
            while (lower < 0) lower += 360;
            while (angle < lower) angle += 360;
            while (upper < lower) upper += 360;

            return (lower <= angle) && (angle <= upper);
        }

        public static int BitFold32(int lower16, int higher16)
        {
            return (lower16) | (higher16 << 16);
        }

        public static void BitUnfold32(int bits32, out int lower16, out int upper16)
        {
            lower16 = (int)(bits32 & UInt16.MaxValue);
            upper16 = (int)(bits32 >> 16);
        }

        public static void BitUnfold64(ulong bits64, out int lower32, out int upper32)
        {
            lower32 = (int)(bits64 & UInt32.MaxValue);
            upper32 = (int)(bits64 >> 32);
        }

        public static void GetWalkCoordinate(int x, int y, Direction dir, out int dx, out int dy)
        {
            //GETCHANGECOORDINATE?
            //ADDIIONAL PARAM FOR speed? WALK/RUN/MOUNT
            if (dir == Direction.North)
            {
                x--;
                y--;
            }
            else if (dir == Direction.NorthEast) 
            {
                y--;
            }
            else if (dir == Direction.East)
            {
                x++;
                y--;
            }
            else if (dir == Direction.SouthEast) 
            {
                x++;
            }
            else if (dir == Direction.South)
            {
                x++;
                y++;
            }
            else if (dir == Direction.SouthWest)
            {
                y++;
            }
            else if (dir == Direction.West)
            {
                x--;
                y++;
            }
            else if (dir == Direction.NorthWest)
            {
                x--;
            }
            dx = x;
            dy = y;
        }
    }
}
