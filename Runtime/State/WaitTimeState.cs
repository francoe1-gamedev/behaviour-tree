using BehaviourTree.Core;
using BehaviourTree.Interfaces;
using BehaviourTree.State;
using UnityEngine;

namespace BehaviourTree.State
{
    public class WaitTimeState : Node, INodeEditor
    {
        public float Value { get; set; }

        private float _time { get; set; }

        private NodeProperty[] _properties { get; } = new NodeProperty[1];

        public WaitTimeState()
        {
            _properties[0] = new NodeProperty("Time", () => Value, x => Value = (float)x);
        }

        protected override void OnEnter(INode node)
        {
            _time = Value;
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

        NodeProperty[] INodeEditor.GetProperties() => _properties;

        protected override void OnReset()
        {
        }
    }
}

namespace BehaviourTree
{
    public partial class BehaviourTreeBuilder
    {
        public BehaviourTreeBuilder Wait(float time)
        {
            WaitTimeState state = Node.Create<WaitTimeState>();
            state.Value = time;
            Add(state);
            return this;
        }
    }
}