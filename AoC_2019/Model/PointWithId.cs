namespace AoC_2019.Model
{
    public class PointWithId : AoCHelper.Model.Point
    {
        public string Id { get; set; }

        public PointWithId(string id, int x, int y) : base(x, y)
        {
            Id = id;
        }
    }
}
