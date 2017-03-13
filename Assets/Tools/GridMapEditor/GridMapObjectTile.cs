using Common;
using Common.Grid;
using Game.Grid;
using System;
using UnityEngine;

namespace Tools
{
    [Serializable]
    public struct GridMapObjectTile
    {
        public GridPosition3D Position;

        [EnumFlag(typeof(GridMapObjectTile), "ConvertBlockerEnum")]
        public GridTileBlockerFlags TileBlockerSettings;
        [EnumFlag()]
        public GameGridObjectSettings TileSettings;

        private static int ConvertBlockerEnum(int i_Value, bool i_Export)
        {
            if(i_Export)
            {
                return GridHelpers.BlockerValueToIndexFlags(i_Value);
            }
            return GridHelpers.BlockerIndexFlagsToValue(i_Value);
        }

    }
}
