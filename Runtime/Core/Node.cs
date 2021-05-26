using BehaviourTree.Interfaces;
using System;
using System.Text.RegularExpressions;

namespace BehaviourTree.Core
{
    public abstract class Node : INode
    {
        public NodeState State { get; private set; }
        public NodeExcecuteState ExecutionState { get; private set; }

        private int _id { get; set; }
        private static int _nextId { get; set; }
        private string _name { get; set; }

        public event Action<NodeState> ChangeStateEvent;

        public event Action<NodeExcecuteState> ChangeExecutionStateEvent;

        protected static int GetNextId() => ++_nextId;

        int INode.Id => _id;

        private void Initialize()
        {
            _nextId += 1;
            _id = GetNextId();
            _name = Regex.Replace(GetType().Name.Replace("State", ""), @"([A-Z])", " $1").Trim();
        }

        private void SetState(NodeState state)
        {
            State = state;
            ChangeStateEvent?.Invoke(state);
        }

        private void SetExecutionState(NodeExcecuteState state)
        {
            ExecutionState = state;
            ChangeExecutionStateEvent?.Invoke(state);
        }

        void INode.Enter(INode enter)
        {
            SetState(NodeState.Enter);
            OnEnter(enter);
        }

        void INode.Tick()
        {
            if (State == NodeState.Enter) SetState(NodeState.Run);
            if (State == NodeState.Run)
            {
                SetExecutionState(OnTick());
                if (ExecutionState != NodeExcecuteState.Continue)
                    SetState(NodeState.Completed);
            }
        }

        void INode.Exit()
        {
            SetState(NodeState.Exit);
            OnExit();
        }

        void INode.Stop()
        {
            if (State != NodeState.Run) return;
            SetState(NodeState.Stop);
            OnStop();
        }

        void INode.Reset()
        {
            OnReset();
            SetState(NodeState.Exit);
            SetExecutionState(NodeExcecuteState.Continue);
            if (this is INodeCollection collection)
                foreach (INode node in collection.GetNodes())
                    node.Reset();
        }

        string INode.GetName() => _name;

        void INode.SetName(string value) => _name = value;

        string INode.GetDescription() => GetDescription();

        NodeState INode.GetState() => State;

        protected abstract void OnEnter(INode node);

        protected abstract NodeExcecuteState OnTick();

        protected abstract void OnExit();

        protected abstract void OnStop();

        protected abstract void OnReset();

        protected virtual string GetDescription() => null;

        bool INode.Compare(INode node) => node != default && _id == node.Id;

        NodeExcecuteState INode.GetExecutionState() => ExecutionState;

        public static T Create<T>() where T : Node, new()
        {
            T reference = new T();
            reference.Initialize();
            return reference;
        }
    }
}