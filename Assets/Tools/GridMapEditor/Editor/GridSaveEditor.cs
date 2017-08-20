using Common.Grid.Serialization;
using Common.Grid.Serialization.Specialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    [CustomEditor(typeof(GridSave3D))]
    public class GridSave3DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                DrawDefaultInspector();
            }
            
            var gridSave = (GridSave3D)target;
            int tileCount = 0;
            if(gridSave.TileDataList != null)
            {
                tileCount = gridSave.TileDataList.Count;
            }
            EditorGUILayout.LabelField("TileCount", tileCount.ToString());
        }
    }
}
