using BehaviourTree.Core;
using BehaviourTree.Interfaces;
using BehaviourTree.State;
using System;

namespace BehaviourTree.State
{
    public class UntilBooleanState : Node
    {
        public Func<bool> Value { get; set; }

        public bool ValueTarget { get; set; }

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

        protected override NodeExcecuteState OnTick() => Value() == ValueTarget ? NodeExcecuteState.Success : NodeExcecuteState.Continue;
    }
}

namespace BehaviourTree
{
    public partial class BehaviourTreeBuilder
    {
        public BehaviourTreeBuilder UntilTrue(Func<bool> condition) => UntilBoolean(condition, true);

        public BehaviourTreeBuilder UntilFalse(Func<bool> condition) => UntilBoolean(condition, false);

        private BehaviourTreeBuilder UntilBoolean(Func<bool> condition, bool target)
        {
            UntilBooleanState state = Node.Create<UntilBooleanState>();
            state.Value = condition;
            state.ValueTarget = target;
            Add(state);
            return this;
        }
    }
}