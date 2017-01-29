using UnityEditor;
using UnityEngine;


namespace Common.Text
{
    [CustomPropertyDrawer(typeof(HashedString))]
    public class HashedStringDrawer : PropertyDrawer
    {
        private Rect m_ContentArea;

        // Draw the property inside the given rect
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            m_ContentArea = pos;
            EditorGUI.BeginProperty(m_ContentArea, label, prop);
            m_ContentArea.height = EditorGUIExtensions.DEFAULT_GUI_HEIGHT;

            SerializedProperty textProperty = prop.FindPropertyRelative("Text");
            if (textProperty != null)
            {
                string newText = EditorGUI.TextField(m_ContentArea, "Text", textProperty.stringValue);
                textProperty.stringValue = newText;
                SerializedProperty hashProperty = prop.FindPropertyRelative("Hash");
                if (hashProperty != null)
                {
                    hashProperty.intValue = string.IsNullOrEmpty(newText) ? 0 : newText.GetHashCode();
                }
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return EditorGUIExtensions.DEFAULT_GUI_LINE_HEIGHT;
        }
    }
}
