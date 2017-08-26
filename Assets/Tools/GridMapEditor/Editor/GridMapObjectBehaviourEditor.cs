
using System.Collections.Generic;
using Tools.Specialization;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Tools
{
    [CustomEditor(typeof(GridMapObjectCuboidBehaviour))]
    public class GridMapObjectBehaviourEditor : Editor
    {
        private ReorderableList m_GridTileList;
        private ReorderableList m_GridConnectionsList;
        private GridMapObjectCuboidBehaviour m_TargetObject;

        // Use this for initialization
        public void OnEnable()
        {
            m_GridTileList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("m_Tiles"),
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

            m_TargetObject = serializedObject.targetObject as GridMapObjectCuboidBehaviour;
            GridMapObjectCuboidBehaviour.ActiveGridObject = m_TargetObject.GetInstanceID();
            m_TargetObject.DrawDebug();
        }

        private void OnDisable()
        {
            if(GridMapObjectCuboidBehaviour.ActiveGridObject == m_TargetObject.GetInstanceID())
            {
                GridMapObjectCuboidBehaviour.ActiveGridObject = -1;
                GridMapObjectCuboidBehaviour.ActiveGridTileIndex = -1;
                m_TargetObject.DrawDebug();
            }
        }

        private void UpdateSelectDraw(ReorderableList list)
        {
            int oldObjectId = GridMapObjectCuboidBehaviour.ActiveGridObject;
            int oldTileIndex = GridMapObjectCuboidBehaviour.ActiveGridTileIndex;
            GridMapObjectCuboidBehaviour.ActiveGridObject = m_TargetObject.GetInstanceID();
            GridMapObjectCuboidBehaviour.ActiveGridTileIndex = list.index;
            if (GridMapObjectCuboidBehaviour.ActiveGridObject != oldObjectId || GridMapObjectCuboidBehaviour.ActiveGridTileIndex != oldTileIndex)
            {
                m_TargetObject.DrawDebug();
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
            DrawDefaultInspector();
            serializedObject.Update();
            
            m_GridTileList.DoLayoutList();
            m_GridConnectionsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
        
    }
}
