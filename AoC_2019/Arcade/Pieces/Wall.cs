namespace AoC_2019.Arcade.Pieces
{
    public class Wall : Piece
    {
        public static int Id => 1;

        public override char Render() => Position.Y == 0 ? '—' : '|';
    }
}
