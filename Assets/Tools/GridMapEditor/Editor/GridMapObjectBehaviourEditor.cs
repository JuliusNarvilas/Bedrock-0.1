
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Tools
{
    [CustomEditor(typeof(GridMapObjectBehaviour))]
    public class GridMapObjectBehaviourEditor : Editor
    {
        private ReorderableList m_GridObjectList;
        private List<SerializedProperty> m_OtherProperties;
        private GridMapObjectBehaviour m_TargetObject;

        // Use this for initialization
        public void OnEnable()
        {
            m_GridObjectList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Tiles"),
                true, true, true, true);

            m_GridObjectList.elementHeightCallback = GetElementHeight;
            m_GridObjectList.drawElementCallback = DrawGridObject;
            m_GridObjectList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Tiles", EditorStyles.boldLabel);
            };
            m_GridObjectList.onSelectCallback = UpdateSelectDraw;
            
            m_OtherProperties = new List<SerializedProperty> {
                serializedObject.FindProperty("Id"),
                serializedObject.FindProperty("Offset"),
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

        private float GetElementHeight(int index)
        {
            var temp = m_GridObjectList.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(temp);
        }

        private void DrawGridObject(Rect rect, int index, bool isActive, bool isFocused)
        {
            const int GridObjectDisplayPadding = 2;
            rect.y += GridObjectDisplayPadding;

            var element = m_GridObjectList.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(rect, element, true);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            foreach(var property in m_OtherProperties)
            {
                EditorGUILayout.PropertyField(property);
            }
            
            m_GridObjectList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            m_TargetObject.SnapToGrid(true);
        }
        
    }
}
