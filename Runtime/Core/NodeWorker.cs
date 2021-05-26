using BehaviourTree.Interfaces;

namespace BehaviourTree.Core
{
    public class NodeWorker
    {
        private INode _root { get; set; }
        private INode _current { get; set; }

        public INode Root => _root;

        public NodeWorker()
        {
        }

        public NodeWorker(INode node) => SetNode(node);

        public void SetNode(INode node)
        {
            _root = node;
            _current = node;
        }

        public bool Update(INode from)
        {
            if (_current == null) return true;
            if (_current.GetState() == NodeState.Exit || _current.GetState() == NodeState.Stop) _current.Enter(from);
            if (_current.GetState() == NodeState.Enter || _current.GetState() == NodeState.Run) _current.Tick();
            if (_current.GetState() == NodeState.Completed) Move(GetNextState());
            return _current == null;
        }

        public void Reset()
        {
            _current = _root;
        }

        private INode GetNextState()
        {
            if (_current is INodeTransition nodeTransition)
                return nodeTransition.GetNextNode();
            return default;
        }

        private void Move(INode next)
        {
            _current?.Exit();
            next?.Enter(_current);
            _current = next;
        }
    }
}