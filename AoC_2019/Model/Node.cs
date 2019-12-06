using System.Collections.Generic;
using System.Linq;

namespace AoC_2019.Model
{
    public class Node : AoCHelper.Model.Node<string>
    {
        public ICollection<Node> Children { get; set; }

        public Node(string id) : base(id)
        {
            Children = new List<Node>();
        }

        public Node(string id, Node child) : base(id)
        {
            Children = new List<Node>() { child };
        }

        public int GrandChildrenCount()
        {
            return Children.Count + Children.Select(child => child.GrandChildrenCount()).Sum();
        }

        public int RelationShipCount()
        {
            return Children.Count
                   + Children.Select(child => child.GrandChildrenCount()).Sum()
                   + Children.Select(child => child.RelationShipCount()).Sum();
        }
    }
}
