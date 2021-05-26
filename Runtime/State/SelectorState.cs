using BehaviourTree.Core;
using BehaviourTree.Interfaces;
using BehaviourTree.State;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree.State
{
    public class SelectorState : Node, INodeCollection, INodeLoopeable
    {
        private List<INode> _nodes { get; } = new List<INode>();
        private int _current { get; set; } = 0;
        private NodeWorker _worker { get; } = new NodeWorker();
        public LoopMode Loop { get; set; }

        private bool _forward { get; set; } = true;

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
            if (_worker.Update(this))
            {
                if (!MoveNext())
                {
                    if (Loop == LoopMode.None) return GetExitState();
                    MoveFirst();
                }
            }
            return NodeExcecuteState.Continue;
        }

        private NodeExcecuteState GetExitState()
        {
            foreach (INode node in _nodes)
                if (node.GetExecutionState() == NodeExcecuteState.Success)
                    return NodeExcecuteState.Success;
            return NodeExcecuteState.Fail;
        }

        private bool MoveNext()
        {
            bool value = Move(_forward ? _current + 1 : _current - 1);

            switch (Loop)
            {
                case LoopMode.Default:
                    if (_nodes.Count - 1 == _current) _current = -1;
                    break;

                case LoopMode.Pinpong:
                    if (_forward && _current == _nodes.Count - 1) _forward = false;
                    if (!_forward && _current == 0) _forward = true;
                    break;
            }

            return value;
        }

        private bool MoveFirst()
        {
            _forward = true;
            return Move(0);
        }

        private bool Move(int index)
        {
            _current = index;
            if (_nodes.Count <= _current) return false;
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
        public static BehaviourTreeBuilder Selector(GameObject go)
        {
            BehaviourTreeBuilder instance = new BehaviourTreeBuilder(go);
            instance.Selector();
            return instance;
        }

        /// <summary> OR </summary>
        public BehaviourTreeBuilder Selector()
        {
            ForwardStack(Node.Create<SelectorState>());
            return this;
        }
    }
}