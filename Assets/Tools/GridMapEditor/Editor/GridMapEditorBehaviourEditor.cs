using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Tools
{
    /*
    [CustomEditor(typeof(GridMapEditorBehaviour))]
    public class GridMapEditorBehaviourEditor : Editor
    {
        private SerializedProperty m_SizeX;
        private SerializedProperty m_SizeY;
        private SerializedProperty m_GridType;

        // Use this for initialization
        public void OnEnable()
        {
            m_SizeX = serializedObject.FindProperty("m_SizeX");
            m_SizeY = serializedObject.FindProperty("m_SizeY");
            m_GridType = serializedObject.FindProperty("m_GridType");
        }

        private void DrawGridObject(Rect rect, int index, bool isActive, bool isFocused)
        {
            const int GridObjectDisplayPadding = 2;
            rect.y += GridObjectDisplayPadding;
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Label("Grid Map Properties", EditorStyles.boldLabel);
            
            //EditorGUILayout

            serializedObject.ApplyModifiedProperties();
        }
    }
    */
}