
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Tools
{
    [CustomEditor(typeof(GridMapObjectBehaviour))]
    public class GridMapObjectBehaviourEditor : Editor
    {
        private ReorderableList m_GridTileList;
        private ReorderableList m_GridConnectionsList;
        private List<SerializedProperty> m_OtherProperties;
        private GridMapObjectBehaviour m_TargetObject;

        // Use this for initialization
        public void OnEnable()
        {
            m_GridTileList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Tiles"),
                true, true, true, true);

            m_GridTileList.elementHeightCallback = GetTileHeight;
            m_GridTileList.drawElementCallback = DrawTile;
            m_GridTileList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Tiles", EditorStyles.boldLabel);
            };
            m_GridTileList.onSelectCallback = UpdateSelectDraw;

            m_GridConnectionsList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Connections"),
                true, true, true, true);

            m_GridConnectionsList.elementHeightCallback = GetConnectionHeight;
            m_GridConnectionsList.drawElementCallback = DrawConnection;
            m_GridConnectionsList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Connections", EditorStyles.boldLabel);
            };
            //m_GridConnectionsList.onSelectCallback = UpdateSelectDraw;

            m_OtherProperties = new List<SerializedProperty> {
                serializedObject.FindProperty("Id"),
                serializedObject.FindProperty("Position"),
                serializedObject.FindProperty("Size"),
                serializedObject.FindProperty("ObjectSettings")
            };

            m_TargetObject = serializedObject.targetObject as GridMapObjectBehaviour;
        }

        private void UpdateSelectDraw(ReorderableList list)
        {
            int oldObjectId = GridMapObjectBehaviour.ActiveGridObject;
            int oldTileIndex = GridMapObjectBehaviour.ActiveGridTileIndex;
            GridMapObjectBehaviour.ActiveGridObject = m_TargetObject.GetInstanceID();
            GridMapObjectBehaviour.ActiveGridTileIndex = list.index;
            if (GridMapObjectBehaviour.ActiveGridObject != oldObjectId || GridMapObjectBehaviour.ActiveGridTileIndex != oldTileIndex)
            {
                SceneView.RepaintAll();
            }
        }

        private float GetTileHeight(int index)
        {
            var temp = m_GridTileList.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(temp);
        }
        private float GetConnectionHeight(int index)
        {
            var temp = m_GridConnectionsList.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(temp);
        }

        private void DrawTile(Rect rect, int index, bool isActive, bool isFocused)
        {
            const int GridObjectDisplayPadding = 2;
            rect.y += GridObjectDisplayPadding;

            var element = m_GridTileList.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(rect, element, true);
        }
        private void DrawConnection(Rect rect, int index, bool isActive, bool isFocused)
        {
            const int GridObjectDisplayPadding = 2;
            rect.y += GridObjectDisplayPadding;

            var element = m_GridConnectionsList.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(rect, element, true);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            foreach(var property in m_OtherProperties)
            {
                EditorGUILayout.PropertyField(property);
            }
            
            m_GridTileList.DoLayoutList();
            m_GridConnectionsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
        
    }
}
