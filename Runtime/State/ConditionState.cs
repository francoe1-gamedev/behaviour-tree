using BehaviourTree.Core;
using BehaviourTree.Interfaces;
using BehaviourTree.States;
using System;

namespace BehaviourTree.States
{
    public class ConditionState : Node
    {
        public Func<NodeExcecuteState> Value { get; set; }

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

        protected override NodeExcecuteState OnTick() => Value();
    }
}

namespace BehaviourTree
{
    public partial class BehaviourTreeBuilder
    {
        public BehaviourTreeBuilder Condition(Func<NodeExcecuteState> condition)
        {
            ConditionState state = Node.Create<ConditionState>();
            state.Value = condition;
            Add(state);
            return this;
        }

        public BehaviourTreeBuilder AssertTrue(Func<bool> value)
        {
            ConditionState state = Node.Create<ConditionState>();
            state.Value = () => value() ? NodeExcecuteState.Success : NodeExcecuteState.Fail;
            Add(state);
            return this;
        }

        public BehaviourTreeBuilder AssertFalse(Func<bool> value)
        {
            ConditionState state = Node.Create<ConditionState>();
            state.Value = () => !value() ? NodeExcecuteState.Success : NodeExcecuteState.Fail;
            Add(state);
            return this;
        }
    }
}