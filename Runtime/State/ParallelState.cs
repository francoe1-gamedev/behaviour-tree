using BehaviourTree.Core;
using BehaviourTree.Interfaces;
using BehaviourTree.State;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourTree.State
{
    public enum ParallelMode
    {
        ExitAllSuccess,
        ExitWithFirstSuccess,
        ExitWithFirstFail,
        ExitFirstComplete,
    }

    public class ParallelState : Node, INodeCollection, INodeLoopeable
    {
        private List<INode> _nodes { get; } = new List<INode>();
        private List<NodeWorker> _workers { get; } = new List<NodeWorker>();
        public LoopMode Loop { get; set; }

        public ParallelMode Mode { get; set; }

        IEnumerable<INode> INodeCollection.GetNodes() => _nodes;

        public void Add(INode node)
        {
            if (node.Compare(this))
            {
                Debug.LogWarning("Recursive State");
                return;
            }

            _nodes.Add(node);
            _workers.Add(new NodeWorker(node));
        }

        protected override void OnEnter(INode node)
        {
            foreach (NodeWorker worker in _workers)
                worker.Reset();

            foreach (INode item in _nodes)
                item.Reset();
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

            int completed = 0;
            foreach (NodeWorker worker in _workers)
            {
                if (worker.Update(this))
                {
                    if (Mode == ParallelMode.ExitWithFirstSuccess)
                    {
                        if (worker.Root.GetExecutionState() == NodeExcecuteState.Success)
                            return NodeExcecuteState.Success;
                    }
                    else if (Mode == ParallelMode.ExitWithFirstFail)
                    {
                        if (worker.Root.GetExecutionState() == NodeExcecuteState.Fail)
                            return NodeExcecuteState.Fail;
                    }
                    else if (Mode == ParallelMode.ExitFirstComplete)
                    {
                        return worker.Root.GetExecutionState();
                    }

                    completed++;
                    if (Loop != LoopMode.None) worker.Reset();
                }
            }

            if (Loop != LoopMode.None) return NodeExcecuteState.Continue;
            else if (completed < _workers.Count) return NodeExcecuteState.Continue;
            return NodeExcecuteState.Success;
        }

        protected override string GetDescription()
        {
            int finish = _nodes.Count(x => x.GetState() == NodeState.Exit);
            return $"Process {finish}/{_workers.Count}";
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
        public static BehaviourTreeBuilder Parallel(GameObject go)
        {
            BehaviourTreeBuilder instance = new BehaviourTreeBuilder(go);
            instance.Parallel();
            return instance;
        }
        public BehaviourTreeBuilder Parallel(Action<ParallelState> setup = null)
        {
            ParallelState state = Node.Create<ParallelState>();
            setup?.Invoke(state);
            ForwardStack(state);
            return this;
        }
    }
}