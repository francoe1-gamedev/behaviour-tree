using BehaviourTree.Core;
using UnityEngine;

namespace BehaviourTree
{
    public partial class BehaviourTreeSchema
    {
        internal class NodeSchemaRenderData
        {
            public Rect Rect => new Rect(Position, Size);
            public Vector2 Size { get; private set; } = new Vector2(BehaviourTreeViewer.NODE_WIDTH, BehaviourTreeViewer.NODE_HEIGHT);
            public Vector2 Position { get; private set; }
            public AnimatedColor BackgroundColor { get; } = new AnimatedColor(Color.black);
            public AnimatedColor TransitionColor { get; } = new AnimatedColor(Color.white);

            internal NodeSchemaRenderData(NodeSchema schema)
            {
                if (schema.Node is Node node)
                {
                    node.ChangeStateEvent += (state) => UpdateColor(schema);
                    node.ChangeExecutionStateEvent += (state) => UpdateColor(schema);
                }
            }

            private void UpdateColor(NodeSchema schema)
            {
                BackgroundColor.Value = GetBackgroundColor(schema);
                TransitionColor.Value = GetTransitionColor(schema.Parent, schema);
            }

            public Vector2 GetTopCenter()
            {
                Vector2 position = Position;
                position.x += Size.x / 2;
                return position;
            }

            public Vector2 GetBottomCenter()
            {
                Vector2 position = GetTopCenter();
                position.y += Size.y;
                return position;
            }

            public void SetPosition(Vector2 position)
            {
                Position = position;
            }

            private static Color GetBackgroundColor(NodeSchema node)
            {
                switch (node.Node.GetState())
                {
                    case NodeState.Stop: return BehaviourTreeSetupValue.STOP_COLOR;
                    case NodeState.Run: return BehaviourTreeSetupValue.RUN_COLOR;
                    case NodeState.Exit:
                        switch (node.Node.GetExecutionState())
                        {
                            case NodeExcecuteState.Success: return BehaviourTreeSetupValue.SUCCESS_COLOR;
                            case NodeExcecuteState.Fail: return BehaviourTreeSetupValue.FAIL_COLOR;
                        }
                        break;
                }
                return BehaviourTreeSetupValue.EXIT_COLOR;
            }

            private static Color GetTransitionColor(NodeSchema from, NodeSchema to)
            {
                if (from == null) return BehaviourTreeSetupValue.EXIT_COLOR;
                if (to == null) return BehaviourTreeSetupValue.EXIT_COLOR;

                if (from.Node.GetState() == NodeState.Run && to.Node.GetState() == NodeState.Run) return BehaviourTreeSetupValue.RUN_COLOR;
                if (from.Node.GetState() == NodeState.Completed && to.Node.GetState() == NodeState.Completed)
                    return to.Node.GetExecutionState() == NodeExcecuteState.Fail ? BehaviourTreeSetupValue.FAIL_COLOR : BehaviourTreeSetupValue.SUCCESS_COLOR;

                if (to.Node.GetExecutionState() == NodeExcecuteState.Fail) return BehaviourTreeSetupValue.FAIL_COLOR;
                if (to.Node.GetState() == NodeState.Exit && to.Node.GetExecutionState() != NodeExcecuteState.Continue)
                    return to.Node.GetExecutionState() == NodeExcecuteState.Fail ? BehaviourTreeSetupValue.FAIL_COLOR : BehaviourTreeSetupValue.SUCCESS_COLOR;

                if (to.Node.GetState() == NodeState.Stop) return BehaviourTreeSetupValue.STOP_COLOR;
                return BehaviourTreeSetupValue.EXIT_COLOR;
            }
        }
    }

    public class AnimatedColor
    {
        private Color _color { get; set; }

        private Color _targetColor { get; set; }

        public Color Value
        {
            get => _targetColor = Color.Lerp(_targetColor, _color, BehaviourTreeSetupValue.ANIMATION_SPEED);
            set => _color = value;
        }

        public AnimatedColor(Color color)
        {
            _color = color;
            _targetColor = color;
        }

        public void SetRaw(Color color)
        {
            _color = color;
            _targetColor = color;
        }
    }
}