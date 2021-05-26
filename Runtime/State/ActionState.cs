using BehaviourTree.Core;
using BehaviourTree.Interfaces;
using BehaviourTree.State;
using System;

namespace BehaviourTree.State
{
    public class ActionState : Node
    {
        public Action Value { get; set; }

        protected override void OnEnter(INode node)
        {
        }

        protected override void OnExit()
        {
        }

        protected override void OnReset()
        {
        }

        protected override void OnStop()
        {
        }

        protected override NodeExcecuteState OnTick()
        {
            Value();
            return NodeExcecuteState.Success;
        }
    }
}

namespace BehaviourTree
{
    public partial class BehaviourTreeBuilder
    {
        public BehaviourTreeBuilder Action(Action action)
        {
            ActionState state = Node.Create<ActionState>();
            state.Value = action;
            Add(state);
            return this;
        }
    }
}