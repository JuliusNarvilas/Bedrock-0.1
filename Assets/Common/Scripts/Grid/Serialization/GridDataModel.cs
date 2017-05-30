using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common.Grid.Serialization
{
    public class GridModelTileData
    {
        public GridPosition3D Position;
        public int Settings;
    }

    [Serializable]
    public class GridDataModel
    {
        public Vector3 Translation;
        public Quaternion Orientation;
        public Vector3 Scale;

        public Mesh Mesh;
        //TODO:
        public ScriptableObject Logic;
        public List<GridModelTileData> TileData;
    }
}
