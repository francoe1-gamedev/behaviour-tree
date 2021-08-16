using BehaviourTree.Core;
using BehaviourTree.Decorator;
using BehaviourTree.Interfaces;
using System;
using System.Collections.Generic;

namespace BehaviourTree.Decorator
{
    public enum RepeatMode
    {
        Fail,
        Infinite,
        Success,
    }
    public class RepeaterDecorator : Node, INodeCollection
    {
        private INode _node { get; set; }
        private NodeWorker _worker { get; set; }
        public RepeatMode Mode { get; set; }

        public void Add(INode node)
        {
            if (_node != null) throw new Exception("Repeat accept 1 state");
            _node = node;
            _worker = new NodeWorker(_node);
        }

        public IEnumerable<INode> GetNodes()
        {
            if (_node != null) yield return _node;
        }

        protected override void OnEnter(INode node)
        {
            _node.Reset();
            _worker?.Reset();
        }

        protected override void OnExit()
        {
            _node?.Stop();
        }

        protected override void OnReset()
        {
        }

        protected override void OnStop()
        {
            OnExit();
        }

        protected override NodeExcecuteState OnTick()
        {
            if (_worker?.Update(this) ?? false)
            {
                if (Mode == RepeatMode.Fail && _worker.Root.GetExecutionState() == NodeExcecuteState.Fail) return NodeExcecuteState.Fail;
                if (Mode == RepeatMode.Success && _worker.Root.GetExecutionState() == NodeExcecuteState.Success) return NodeExcecuteState.Success;
                _worker.Reset();
            }
            return NodeExcecuteState.Continue;
        }
    }
}

namespace BehaviourTree
{
    public partial class BehaviourTreeBuilder
    {
        public BehaviourTreeBuilder Repeater()
        {
            RepeaterDecorator state = Node.Create<RepeaterDecorator>();
            ForwardStack(state);
            AutoBack();
            return this;
        }

        public BehaviourTreeBuilder Repeater(Action<RepeaterDecorator> callback)
        {
            Repeater();
            callback((RepeaterDecorator)_lastNode);
            return this;
        }
    }
}