using System;

namespace AoC_2019.Arcade.Pieces
{
    public abstract class UniquePiece : Piece
    {
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

        public override bool Equals(Piece other)
        {
            if (other == null)
            {
                return false;
            }

            return Type.Equals(other.Type);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type);  // Always unique
        }

        #endregion
    }
}
