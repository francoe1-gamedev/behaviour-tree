using UnityEngine;

namespace BehaviourTree.Core
{
    public abstract class NodeGameObject : Node
    {
        public GameObject Owner { get; private set; }

        public static T Create<T>(GameObject owner) where T : NodeGameObject, new()
        {
            T node = Create<T>();
            node.Owner = owner;
            return node;
        }
    }
}