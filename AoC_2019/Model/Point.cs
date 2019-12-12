using System;

namespace AoC_2019.Model
{
    /// <summary>
    /// Simple 3D point class, with equals method and equality operators overriden
    /// </summary>
    public class Point : IEquatable<Point>
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public Point(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]";
        }

        #region Equals override
        // https://docs.microsoft.com/en-us/visualstudio/code-quality/ca1815-override-equals-and-operator-equals-on-value-types?view=vs-2017

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Point))
            {
                return false;
            }

            return Equals((Point)obj);
        }

        public bool Equals(Point other)
        {
            if (other == null)
            {
                return false;
            }

            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public static bool operator ==(Point point1, Point point2)
        {
            if (point1 is null)
            {
                return point2 is null;
            }

            return point1.Equals(point2);
        }

        public static bool operator !=(Point point1, Point point2)
        {
            if (point1 is null)
            {
                return point2 is object;
            }

            return !point1.Equals(point2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        #endregion
    }
}
