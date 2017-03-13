
using UnityEditor;
using UnityEngine;

namespace Common.Grid
{
    [CustomPropertyDrawer(typeof(GridPosition3D))]
    public class GridPosition3DDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var x = prop.FindPropertyRelative("X");
            var y = prop.FindPropertyRelative("Y");
            var z = prop.FindPropertyRelative("Z");
            Vector3 vecValue = new Vector3(x.intValue, y.intValue, z.intValue);

            vecValue = EditorGUI.Vector3Field(position, label, vecValue);

            x.intValue = (int)vecValue.x;
            y.intValue = (int)vecValue.y;
            z.intValue = (int)vecValue.z;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label);
        }
    }
}
