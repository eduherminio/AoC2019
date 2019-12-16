namespace AoC_2019.Arcade.Pieces
{
    public class EmptyPiece : Piece
    {
        public override char Render() => ' ';

        public EmptyPiece(Piece piece)
        {
            Position = piece.Position;
        }
    }
}
