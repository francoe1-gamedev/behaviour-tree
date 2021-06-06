using BehaviourTree.Core;
using BehaviourTree.Interfaces;
using BehaviourTree.State;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree.State
{
    public class SequenceState : Node, INodeCollection
    {
        private List<INode> _nodes { get; } = new List<INode>();
        private int _current { get; set; } = 0;
        private NodeWorker _worker { get; } = new NodeWorker();
        IEnumerable<INode> INodeCollection.GetNodes() => _nodes;

        public void Add(INode node)
        {
            if (node.Compare(this))
            {
                Debug.LogWarning("Recursive State");
                return;
            }

            _nodes.Add(node);
        }

        protected override void OnEnter(INode node)
        {
            foreach (INode item in _nodes)
                item.Reset();
            MoveFirst();
        }

        protected override void OnExit()
        {
            foreach (INode node in _nodes)
                node.Stop();
        }

        protected override void OnStop()
        {
            OnExit();
        }

        protected override NodeExcecuteState OnTick()
        {
            if (_nodes.Count == 0) return NodeExcecuteState.Success;


            if (_worker.Update(this))
            {
                if (_worker.Root.GetExecutionState() != NodeExcecuteState.Success) return NodeExcecuteState.Fail;
                if (!MoveNext()) return NodeExcecuteState.Success;
            }
            return NodeExcecuteState.Continue;
        }

        private bool MoveNext() => Move(++_current);

        private bool MoveFirst() => Move(0);

        private bool Move(int index)
        {
            _current = index;
            if (_nodes.Count == _current) return false;
            _worker.SetNode(_nodes[_current]);
            return true;
        }

        protected override string GetDescription()
        {
            return $"Process {Mathf.Clamp(_current + 1, 0, _nodes.Count)}/{_nodes.Count}";
        }

        protected override void OnReset()
        {
        }
    }
}

namespace BehaviourTree
{
    public partial class BehaviourTreeBuilder
    {
        public static BehaviourTreeBuilder Sequence(GameObject go)
        {
            BehaviourTreeBuilder instance = new BehaviourTreeBuilder(go);
            instance.Sequence();
            return instance;
        }

        /// <summary> AND </summary>
        public BehaviourTreeBuilder Sequence()
        {
            ForwardStack(Node.Create<SequenceState>());
            return this;
        }
    }
}