using AoCHelper.Model;
using System;

namespace AoC_2019.Extensions
{
    public static class PointExtensions
    {
        public static double DistanceTo(this Point point, Point otherPoint)
        {
            return Math.Sqrt(
                Math.Pow(otherPoint.X - point.X, 2)
                + Math.Pow(otherPoint.Y - point.Y, 2));
        }
    }
}
