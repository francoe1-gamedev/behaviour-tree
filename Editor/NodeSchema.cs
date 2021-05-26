using BehaviourTree.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourTree
{
    public partial class BehaviourTreeSchema
    {
        public class NodeSchema
        {
            private NodeSchema[] _childrens { get; set; }
            public INode Node { get; }
            public IReadOnlyCollection<NodeSchema> Childrens => _childrens;
            public int Depth { get; private set; }
            public int PositionInDepth { get; private set; }
            public NodeSchema Parent { get; private set; }
            public int Position { get; private set; }
            internal NodeSchemaRenderData Render { get; }

            public NodeSchema(INode node, int depth, int positionInDepth, ICollection<NodeSchema> childrens)
            {
                Node = node;
                Depth = depth;
                PositionInDepth = positionInDepth;
                _childrens = childrens == null ? new NodeSchema[0] : childrens.ToArray();

                for (int i = 0; i < _childrens.Length; i++)
                {
                    _childrens[i].Parent = this;
                    _childrens[i].Position = i;
                }

                Render = new NodeSchemaRenderData(this);
            }

            internal IEnumerable<NodeSchema> GetAllNodes() => GetAllNodesRecursive(this);

            private IEnumerable<NodeSchema> GetAllNodesRecursive(NodeSchema schema)
            {
                yield return schema;
                foreach (NodeSchema item in schema.Childrens)
                    foreach (NodeSchema child in GetAllNodesRecursive(item))
                        yield return child;
            }
        }
    }
}