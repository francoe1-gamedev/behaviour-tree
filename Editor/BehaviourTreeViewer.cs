using BehaviourTree.Interfaces;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree
{
    public partial class BehaviourTreeViewer : EditorWindow
    {
        public const int NODE_SPACE_X = 20;
        public const int NODE_SPACE_Y = 40;
        public const int NODE_WIDTH = 150;
        public const int NODE_HEIGHT = 58;
        public const int NODE_BORDER = 8;
        public const int NODE_LINE = 8;

        private IBehaviourTree _target { get; set; }
        private BehaviourTreeSchema _schema { get; set; }
        private Vector2 _scroll { get; set; }
        private Vector2 _scrollTarget { get; set; }
        private bool _lock { get; set; }

        [MenuItem("Tools/BehaviourTree")]
        [SuppressMessage("", "IDE0051")]
        private static void Init()
        {
            BehaviourTreeViewer w = CreateWindow<BehaviourTreeViewer>();
            w.Show();
            w.titleContent = new GUIContent("BehaviourTree", EditorGUIUtility.IconContent("d_EditCollider").image, "Viewer for Behaviour tree");
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelection;
            OnSelection();
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelection;
        }

        private void OnSelection()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                if (obj.TryGetComponent(out IBehaviourTree fsm))
                {
                    _target = fsm;
                    ChangeSelection();
                }
            }
        }

        private void ChangeSelection()
        {
            if (_lock) return;
            if (_target.GetRoot() == null)
            {
                _target = null;
                return;
            }
            _schema = new BehaviourTreeSchema(_target.GetRoot());
        }

        private void OnGUI()
        {            
            Repaint();
            GUILayout.Label("Behaviour Tree Viewer");

            if (!Application.isPlaying)
            {
                GUILayout.Label("Available on runtime");
                return;
            }

            if (_target == null)
            {
                GUILayout.Label("Select NodeMachineBehaviour");
                OnSelection();
                if (_target == null) return;
            }

            if (_target.GetRoot() == null)
            {
                GUILayout.Label("Not has behaviour");
                return;
            }

            if (_schema == null)
            {
                GUILayout.Label("Not schema");
                _scroll = new Vector2(0, NODE_SPACE_Y);
                return;
            }

            HandleEvent();
            RenderGraph();
            _lock = EditorGUILayout.Toggle("Lock", _lock);
        }

        private void HandleEvent()
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
            {
                _scrollTarget = Vector2.zero;
                Event.current.Use();
            }

            if (Event.current.type == EventType.MouseDrag)
            {
                _scrollTarget += Event.current.delta * .5f;
                Event.current.Use();
            }

            _scroll = Vector2.Lerp(_scroll, _scrollTarget, Time.deltaTime * 6);
        }

        private void RenderGraph()
        {
            Handles.BeginGUI();
            EditorGUI.DrawRect(new Rect(0, 0, Screen.width, Screen.height), BehaviourTreeSetupValue.GRAPH_BACKGROUND_COLOR);

            for (int i = _schema.Nodes.Count - 1; i >= 0; i--)
            {
                BehaviourTreeSchema.NodeSchema node = _schema.Nodes[i];
                if (node.Parent == null) continue;
                RenderConnection(node.Parent, node);
            }

            foreach (BehaviourTreeSchema.NodeSchema node in _schema.Nodes)
            {
                float fw = (NODE_WIDTH + NODE_SPACE_X) * _schema.GetLengthOfDepth(node.Depth);

                Vector2 pos = new Vector2();
                pos.x = ((Screen.width - fw) / 2) + ((NODE_WIDTH + NODE_SPACE_X) * node.PositionInDepth);
                pos.x -= NODE_SPACE_X / 2f;
                pos.y = (NODE_HEIGHT + NODE_SPACE_Y) * node.Depth;
                pos += _scroll;

                node.Render.SetPosition(pos);

                Rect borderRect = node.Render.Rect;
                borderRect.x -= NODE_BORDER / 2;
                borderRect.width += NODE_BORDER;
                borderRect.y -= NODE_BORDER / 2;
                borderRect.height += NODE_BORDER;

                Color borderColor = node.Render.BackgroundColor.Value;
                borderColor.a = BehaviourTreeSetupValue.ALPHA_DEFAULT;
                EditorGUI.DrawRect(borderRect, borderColor);
                EditorGUI.DrawRect(node.Render.Rect, node.Render.BackgroundColor.Value);

                using (new GUILayout.AreaScope(node.Render.Rect))
                {
                    OnRenderNode(node);
                }
            }
            Handles.EndGUI();
        }

        private void OnRenderNode(BehaviourTreeSchema.NodeSchema node)
        {
            using (new GUILayout.HorizontalScope(EditorStyles.miniButton, GUILayout.ExpandWidth(true)))
            {
                GUILayout.Label(node.Node.GetName());
            }
            GUILayout.Label($"{node.Node.GetState()}=>{node.Node.GetExecutionState()}");
            if (node.Node.GetDescription() != null) GUILayout.Label(node.Node.GetDescription());
        }

        private void RenderConnection(BehaviourTreeSchema.NodeSchema from, BehaviourTreeSchema.NodeSchema to)
        {
            bool isCurved = Mathf.Abs(from.Render.GetBottomCenter().x - to.Render.GetBottomCenter().x) > 2f;
            Color borderColor = to.Render.TransitionColor.Value;
            borderColor.a = BehaviourTreeSetupValue.ALPHA_DEFAULT;

            Handles.DrawBezier(
                from.Render.GetBottomCenter() - Vector2.up * 5,
                to.Render.GetTopCenter() + Vector2.up * 5,
                from.Render.GetBottomCenter() + Vector2.up * (isCurved ? (NODE_SPACE_Y * 1.25f) : 1),
                to.Render.GetTopCenter() - Vector2.up * (isCurved ? (NODE_SPACE_Y * 1.25f) : 1),
                borderColor,
                null,
                NODE_LINE
            );

            Handles.DrawBezier(
                from.Render.GetBottomCenter() - Vector2.up * 5,
                to.Render.GetTopCenter() + Vector2.up * 5,
                from.Render.GetBottomCenter() + Vector2.up * (isCurved ? (NODE_SPACE_Y * 1.25f) : 1),
                to.Render.GetTopCenter() - Vector2.up * (isCurved ? (NODE_SPACE_Y * 1.25f) : 1),
                to.Render.TransitionColor.Value,
                null,
                NODE_LINE / 3
            );
        }
    }
}