using BehaviourTree.Core;
using BehaviourTree.Interfaces;
using BehaviourTree.State;
using System;
using UnityEngine;

namespace BehaviourTree.State
{
    public class WaitTimeState : Node
    {
        public Func<float> Value { get; set; }

        private float _time { get; set; }

        protected override void OnEnter(INode node)
        {
            _time = Value();
        }

        protected override void OnExit()
        {
        }

        protected override void OnStop()
        {
        }

        protected override NodeExcecuteState OnTick()
        {
            _time = Mathf.Clamp(_time -= Time.deltaTime, 0, float.MaxValue);
            return _time == 0 ? NodeExcecuteState.Success : NodeExcecuteState.Continue; ;
        }

        protected override string GetDescription() => _time.ToString("F2");

        protected override void OnReset()
        {
        }
    }
}

namespace BehaviourTree
{
    public partial class BehaviourTreeBuilder
    {
        public BehaviourTreeBuilder Wait(Func<float> time)
        {
            WaitTimeState state = Node.Create<WaitTimeState>();
            state.Value = time;
            Add(state);
            return this;
        }
    }
}