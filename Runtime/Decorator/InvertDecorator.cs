using BehaviourTree.Core;
using BehaviourTree.Decorator;
using BehaviourTree.Interfaces;
using System.Collections.Generic;

namespace BehaviourTree.Decorator
{
    public class InvertDecorator : Node, INodeCollection
    {
        private INode _node { get; set; }
        private NodeWorker _worker { get; set; }

        public void Add(INode node)
        {
            if (_node != null) throw new System.Exception("Invert accept 1 state");
            _node = node;
            _worker = new NodeWorker(_node);
        }

        public IEnumerable<INode> GetNodes()
        {
            if (_node != null) yield return _node;
        }

        protected override void OnEnter(INode node)
        {
            _worker?.Reset();
        }

        protected override void OnExit()
        {
        }

        protected override void OnReset()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnStop()
        {
        }

        protected override NodeExcecuteState OnTick()
        {
            if (_worker?.Update(this) ?? false)
            {
                switch (_worker.Root.GetExecutionState())
                {
                    case NodeExcecuteState.Success: return NodeExcecuteState.Fail;
                    case NodeExcecuteState.Fail: return NodeExcecuteState.Success;
                }
            }
            return NodeExcecuteState.Continue;
        }
    }
}

namespace BehaviourTree
{
    public partial class BehaviourTreeBuilder
    {
        public BehaviourTreeBuilder Invert()
        {
            InvertDecorator state = Node.Create<InvertDecorator>();
            ForwardStack(state);
            AutoBack();
            return this;
        }
    }
}