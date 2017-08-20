
using System.Collections.Generic;
using UnityEngine;

namespace Common.Grid.Serialization
{
    public class GridSave<TPosition, TTileData> : ScriptableObject
    {
        public TPosition GridSize;
        public Vector3 PhysicalTileSize;

        [HideInInspector]
        public List<TTileData> TileDataList;
    }
}
