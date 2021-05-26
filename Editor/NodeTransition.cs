using UnityEngine;

namespace BehaviourTree
{
    public partial class BehaviourTreeViewer
    {
        protected struct NodeTransition
        {
            public Vector2 PointA { get; }
            public Vector2 PointB { get; }
            public Color Color { get; }

            public NodeTransition(Vector2 pointA, Vector2 pointB, Color color)
            {
                PointA = pointA;
                PointB = pointB;
                Color = color;
            }
        }
    }
}