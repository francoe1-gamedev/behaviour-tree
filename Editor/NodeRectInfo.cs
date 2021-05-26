using UnityEngine;

namespace BehaviourTree
{
    public partial class BehaviourTreeViewer
    {
        protected struct NodeRectInfo
        {
            public Rect Rect { get; }

            public Rect ElementRect { get; }

            public NodeRectInfo(Rect rect, Rect elementRect)
            {
                Rect = rect;
                ElementRect = elementRect;
            }
        }
    }
}