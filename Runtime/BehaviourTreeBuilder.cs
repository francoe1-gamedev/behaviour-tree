using BehaviourTree.Core;
using BehaviourTree.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BehaviourTree
{
    public partial class BehaviourTreeBuilder
    {
        public INode Root { get; private set; }

        [SuppressMessage("", "IDE0052")]
        private GameObject _owner { get; }

        private List<INode> _stack { get; } = new List<INode>();
        private INode _current => _stack.Count == 0 ? null : _stack[_stack.Count - 1];
        private INode _lastNode { get; set; }
        private int _autoBack { get; set; }

        protected BehaviourTreeBuilder(GameObject owner)
        {
            _owner = owner;
        }

        protected void ForwardStack(INode node)
        {
            if (_current == null) Root = node;
            Add(node);
            _stack.Add(node);
        }

        protected void BackStack()
        {
            _stack.Remove(_current);
        }

        protected void AutoBack()
        {
            _autoBack++;
        }

        public BehaviourTreeBuilder Setup<T>(Action<T> callback) where T : INode
        {
            callback?.Invoke((T)_lastNode);
            return this;
        }              

        public BehaviourTreeBuilder End()
        {
            BackStack();
            return this;
        }

        public BehaviourTreeBuilder SetName(string name, bool overrideType = false)
        {
            if (_lastNode != null) _lastNode.SetName(overrideType ? name : _lastNode.GetName() + " " + name);
            return this;
        }
                
        public BehaviourTreeBuilder Add<T>(T node, Action<T> setup = null) where T : INode
        {
            setup?.Invoke(node);
            if (_current is INodeCollection collection) collection.Add(node);
            _lastNode = node;

            if (_autoBack > 0)
            {
                _autoBack--;
                BackStack();
            }
            return this;
        }
    }
}