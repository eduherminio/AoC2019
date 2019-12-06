using AoCHelper.Model;

namespace AoC_2019.Model
{
    public class Node : TreeNode<string>
    {
        public Node(string id) : base(id)
        {
        }

        public Node(string id, Node child) : base(id, child)
        {
        }
    }
}
