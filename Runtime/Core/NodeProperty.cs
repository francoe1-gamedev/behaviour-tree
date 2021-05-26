using System;

namespace BehaviourTree.Core
{
    public class NodeProperty
    {
        private Func<object> _getCallback { get; set; }
        private Action<object> _setCallback { get; set; }

        public string Name { get; }

        public NodeProperty(string name, Func<object> getCallback, Action<object> setCallback)
        {
            Name = name;
            _getCallback = getCallback;
            _setCallback = setCallback;
        }

        public void Set(object value) => _setCallback(value);

        public object Get() => _getCallback();
    }
}