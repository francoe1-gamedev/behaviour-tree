using System.Collections.Generic;

namespace BehaviourTree.Interfaces
{
    public interface INodeCollection
    {
        IEnumerable<INode> GetNodes();

        void Add(INode node);
    }
}