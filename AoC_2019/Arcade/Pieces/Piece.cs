using AoCHelper.Model;
using System;

namespace AoC_2019.Arcade.Pieces
{
    public abstract class Piece : IEquatable<Piece>
    {
        public string Type => GetType().Name;

        public Point Position { get; set; }

        public void Init(long x, long y)
        {
            Position = new Point((int)x, (int)y);
        }

        public abstract char Render();

        #region Equals override

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Piece))
            {
                return false;
            }

            return Equals((Piece)obj);
        }

        public virtual bool Equals(Piece other)
        {
            if (other == null)
            {
                return false;
            }

            return Position.Equals(other.Position);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Position);
        }

        #endregion
    }
}
