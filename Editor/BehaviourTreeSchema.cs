using BehaviourTree.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourTree
{
    public partial class BehaviourTreeSchema
    {
        public NodeSchema Root { get; }
        private List<NodeSchema> _nodes { get; } = new List<NodeSchema>();
        public IReadOnlyList<NodeSchema> Nodes => _nodes;
        public int Depth { get; private set; }
        private Dictionary<int, List<NodeSchema>> _depthCount { get; } = new Dictionary<int, List<NodeSchema>>();

        public BehaviourTreeSchema(INode root)
        {
            Root = GetNodeSchema(root, 0);
            _nodes = _nodes.OrderBy(x => x.Depth).ToList();
        }

        private NodeSchema GetNodeSchema(INode node, int depth)
        {
            List<NodeSchema> childrens = null;
            if (node is INodeCollection collection)
            {
                childrens = new List<NodeSchema>();
                foreach (INode children in collection.GetNodes())
                    childrens.Add(GetNodeSchema(children, depth + 1));
            }

            NodeSchema schema = new NodeSchema(node, depth, GetLengthOfDepth(depth), childrens);
            if (!_depthCount.ContainsKey(depth)) _depthCount.Add(depth, new List<NodeSchema>());
            _depthCount[depth].Add(schema);
            _nodes.Add(schema);
            return schema;
        }

        public int GetLengthOfDepth(int depth)
        {
            if (!_depthCount.ContainsKey(depth)) return 0;
            return _depthCount[depth].Count;
        }
    }
}