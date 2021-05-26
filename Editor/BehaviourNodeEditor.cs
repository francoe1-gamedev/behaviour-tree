using BehaviourTree.Core;
using BehaviourTree.Interfaces;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree
{
    public static class BehaviourNodeEditor
    {
        public static int GetPropertyLength(INode node)
        {
            if (node is INodeEditor nodeEditor)
                return nodeEditor.GetProperties().Length;
            return 0;
        }

        public static void Render(INode node)
        {
            if (node is INodeEditor nodeEditor)
            {
                NodeProperty[] properties = nodeEditor.GetProperties();
                for (int i = 0; i < properties.Length; i++) RenderField(properties[i]);
            }
        }

        private static void RenderField(NodeProperty property)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(property.Name);
                RenderInput(property);
            }
        }

        private static void RenderInput(NodeProperty property)
        {
            object value = property.Get();

            if (value is float @float) property.Set(EditorGUILayout.FloatField(@float));
            else if (value is int @int) property.Set(EditorGUILayout.IntField(@int));
            else if (value is bool @bool) property.Set(EditorGUILayout.Toggle(@bool));
            else if (value is Vector2 @vector2) property.Set(EditorGUILayout.Vector2Field("", @vector2));
            else if (value is Vector3 @vector3) property.Set(EditorGUILayout.Vector3Field("", @vector3));
            else if (value is Vector3 @vector4) property.Set(EditorGUILayout.Vector4Field("", @vector4));
            else if (value is Color @color) property.Set(EditorGUILayout.ColorField(@color));
            else GUILayout.Box(value.ToString() + "-" + value.GetType().Name);
        }
    }
}