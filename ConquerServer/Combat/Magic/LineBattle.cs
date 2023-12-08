using ConquerServer.Client;
using ConquerServer.Shared;
using ConquerServer.Database.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Combat
{
    using Point = Tuple<int, int>;
    public class LineBattle : MagicBattle
    {
        private enum LineAlgorithm
        {
            DDA,
            Polar,
            Bresenham
        }

        private const LineAlgorithm LINE_ALGO_TYPE = LineAlgorithm.DDA;

        public LineBattle(Entity source, Entity? target, int castX, int castY, MagicTypeModel spell)
            : base(source, target, castX, castY, spell)
        {

        }

      
        protected override void FindTargets()
        {
            if (Spell == null) return;

            // what coordinates are hit by this skill?
            var points = AlgorithmLineEx(Source.X, Source.Y, CastX, CastY, Spell.Range);

            // find all targets along the line
            Targets.AddRange(Source.FieldOfView.Where(p => points.Any(pt => pt.Item1 == p.X && pt.Item2 == p.Y)));
        }

        private void MinhLine()
        {
            /*
            // slope of the attack relative to the caster
            double difY = (Source.Y - CastY);
            double difX = (Source.X - CastX);
            double slope = (difY / difX);


            //but conquer works in whole numbers..
            //round to up/down making a zigzag... fake "line"

            //int largerDif = difY>difX? (int)difY: (int)difX;

            //focus on searching for all coordinates via x
            for (int i =  ; i )
            */
        }

        private static Point[] PolarLine(int x0, int y0, int x1, int y1, int range)
        {
            var theta = Math.Atan2(y1 - y0, x1 - x0);
            if (theta < 0)
                theta += Math.PI * 2;

            var points = new List<Point>();
            Func<double, double, double> distance = (x, y) => Math.Sqrt(((y - y0) * (y - y0)) + ((x - x0) * (x - x0)));
            double cosT = Math.Cos(theta), sinT = Math.Sin(theta);
            var scale = 1.18d;
            var nx = 0d;
            var ny = 0d;
            int lastX = x0, lastY = y0;
            while (Math.Round(distance(lastX, lastY)) <= range)
            {
                points.Add(new Point(lastX, lastY));
                var _x = (double)lastX;
                var _y = (double)lastY;
                while ((int)_x == lastX && (int)_y == lastY)
                {
                    nx += scale;
                    ny += scale;
                    _x = Math.Round(x0 + (nx * cosT));
                    _y = Math.Round(y0 + (ny * sinT));
                }
                lastX = (int)_x;
                lastY = (int)_y;
            }
            return points.ToArray();
        }

        private static Point[] AlgorithmLineEx(int x0, int y0, int x1, int y1, int range)
        {
            if (x0 == x1 && y0 == y1)
                return new Point[0];

            var scale = 1.0f * range / Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0)); //MathHelper.RoughDistance(x0, y0, x1, y1) -- this distance calc is retarded.

            x1 = (int)(0.5f + scale * (x1 - x0) + x0);
            y1 = (int)(0.5f + scale * (y1 - y0) + y0);

            switch (LINE_ALGO_TYPE)
            {
                case LineAlgorithm.DDA:
                    return DDALine(x0, y0, x1, y1);
                case LineAlgorithm.Bresenham:
                    return BresenhamLine(x0, y0, x1, y1);
                case LineAlgorithm.Polar:
                    return PolarLine(x0, y0, x1, y1, range);
                default:
                    throw new InvalidOperationException("Configured line algorithm does not exist");

            }
        }

        private static Point[] BresenhamLine(int x0, int y0, int x1, int y1)
        {
            int __x0 = x0;
            int __y0 = y0;

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;

            // MATH IS HARD OKAY ><

            var points = new List<Point>();
            if ((x0 == __x0 && y0 == __y0) || (steep && y0 == __x0 && x0 == __y0)) // forward line
            {
                for (int x = x0; x <= x1; x++)
                {
                    points.Add(new Point((steep ? y : x), (steep ? x : y)));
                    error = error - dy;
                    if (error < 0)
                    {
                        y += ystep;
                        error += dx;
                    }
                }
            }
            else // backwards line
            {
                var list = new LinkedList<Point>();
                for (int x = x0; x <= x1; x++)
                {
                    list.AddFirst(new Point((steep ? y : x), (steep ? x : y)));
                    error = error - dy;
                    if (error < 0)
                    {
                        y += ystep;
                        error += dx;
                    }
                }

                // go in order
                foreach (var entry in list)
                    points.Add(entry);
            }

            return points.ToArray();
        }


        private static Point[] DDALine(int x0, int y0, int x1, int y1)
        {
            var points = new List<Point>();
            if ((x0 != x1) || (y0 != y1))
            {
                int dx = x1 - x0;
                int dy = y1 - y0;
                int abs_dx = Math.Abs(dx);
                int abs_dy = Math.Abs(dy);
                if (abs_dx > abs_dy)
                {
                    int _0_5 = abs_dx * ((dy > 0) ? 1 : -1);
                    int numerator = dy * 2;
                    int denominator = abs_dx * 2;
                    if (dx > 0)
                    {
                        for (int i = 1; i <= abs_dx; i++)
                        {
                            points.Add(new Point(x0 + i, y0 + (((numerator * i) + _0_5) / denominator)));
                        }
                    }
                    else if (dx < 0)
                    {
                        for (int i = 1; i <= abs_dx; i++)
                        {
                            points.Add(new Point(x0 - i, y0 + (((numerator * i) + _0_5) / denominator)));
                        }
                    }
                }
                else
                {
                    int _0_5 = abs_dy * ((dx > 0) ? 1 : -1);
                    int numerator = dx * 2;
                    int denominator = abs_dy * 2;
                    if (dy > 0)
                    {
                        for (int i = 1; i <= abs_dy; i++)
                        {
                            points.Add(new Point(x0 + (((numerator * i) + _0_5) / denominator), y0 + i));
                        }
                    }
                    else if (dy < 0)
                    {
                        for (int i = 1; i <= abs_dy; i++)
                        {
                            points.Add(new Point(x0 + (((numerator * i) + _0_5) / denominator), y0 - i));
                        }
                    }
                }
            }
            return points.ToArray();
        }
    }
}
