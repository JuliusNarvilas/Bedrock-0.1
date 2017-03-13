
using UnityEditor;
using UnityEngine;

namespace Common.Grid
{
    [CustomPropertyDrawer(typeof(GridPosition2D))]
    public class GridPosition2DDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var target = prop.serializedObject;
            var x = target.FindProperty("X");
            var y = target.FindProperty("Y");
            Vector2 vecValue = new Vector2(x.intValue, y.intValue);

            vecValue = EditorGUI.Vector2Field(position, label, vecValue);

            x.intValue = (int)vecValue.x;
            y.intValue = (int)vecValue.y;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, label);
        }
    }
}
