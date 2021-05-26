namespace BehaviourTree.Interfaces
{
    public interface INodeTransition
    {
        INode GetNextNode();

        void SetNextNode(INode node);
    }
}