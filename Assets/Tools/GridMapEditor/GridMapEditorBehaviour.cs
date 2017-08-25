using Common;
using Common.Graphics;
using Common.Grid;
using Common.Grid.Generation;
using Common.Grid.Serialization;
using Common.Grid.Serialization.Specialization;
using Game.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    [Serializable]
    public struct MaterialReference
    {
        public string Key;
        public Material Mat;
    }

    [ExecuteInEditMode]
    public class GridMapEditorBehaviour<TPosition, TTileSettings> : MonoBehaviour
    {
        public GridSave<TPosition, TTileSettings> SaveAsset;

        public TPosition Size;
        public Vector3 TileSize;
        public bool DrawTileData = true;
        public ScriptableObject TypeData;

        public GridMapEditorTypeData<TPosition, TTileSettings> GetMapTypeData()
        {
            return (GridMapEditorTypeData<TPosition, TTileSettings>) TypeData;
        }

        public void Load()
        {
            if (SaveAsset != null)
            {
                Size = SaveAsset.GridSize;
                TileSize = SaveAsset.PhysicalTileSize;
            }
            else
            {
                Log.ProductionLogWarning("GridMapEditorBehaviour.Load(): No save asset given.");
            }
        }

        public void Save()
        {
            if (SaveAsset != null)
            {
                SaveAsset.GridSize = Size;
                SaveAsset.PhysicalTileSize = TileSize;
            }
            else
            {
                Log.ProductionLogWarning("GridMapEditorBehaviour.Save(): No save asset given.");
            }
        }
        


        private bool m_ValidateUpdate;
        private GameObject m_DebugDisplay;

        private void Update()
        {
            var typeData = GetMapTypeData();
            if (typeData != null && typeData.Updated)
            {
                typeData.Updated = false;
                m_ValidateUpdate = true;
            }
            if (m_ValidateUpdate)
            {
                m_ValidateUpdate = false;
                var gridMapObjects = GetComponentsInChildren<GridMapObjectBehaviour<TPosition, TTileSettings>>();
                foreach (var gridObject in gridMapObjects)
                {
                    gridObject.OnValidate();
                }
                DrawDebug();
            }
        }


        private void DrawDebug()
        {
            if (m_DebugDisplay != null)
            {
                DestroyImmediate(m_DebugDisplay);
            }
            m_DebugDisplay = GetMapTypeData().BuildGridDebug(this);
            m_DebugDisplay.transform.SetParent(transform, false);
        }


        private void OnValidate()
        {
            m_ValidateUpdate = true;
        }
        

        private Material GetMaterialFrom(string key, List<MaterialReference> list)
        {
            Material alternative = null;
            foreach (var obj in list)
            {
                if (obj.Key == key)
                {
                    return obj.Mat;
                }
                else if (obj.Key == "Default")
                {
                    alternative = obj.Mat;
                }
            }
            return alternative;
        }

    }
}