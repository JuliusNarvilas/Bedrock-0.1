using Common;
using Common.Grid;
using Game.Grid;
using System;
using UnityEngine;

namespace Tools
{
    [Serializable]
    public class GridMapObjectTile
    {
        public GridPosition3D Position;

        [EnumFlag(typeof(GridMapObjectTile), "ConvertBlockerEnumToFlags", "ConvertBlockerEnumToValue")]
        public EGridTileSettings TileBlockerSettings;
        [EnumFlag]
        public EGameGridObjectSettings Settings;

        private static int ConvertBlockerEnumToFlags(int i_Value)
        {
            return GridHelpers.BlockerValueToIndexFlags(i_Value);
        }
        private static int ConvertBlockerEnumToValue(int i_Value, int i_NewValue)
        {
            var temp = GridHelpers.BlockerIndexFlagsToValue(i_Value);
            var newForceVals = GridHelpers.BlockerIndexFlagsToValue(i_NewValue);
            int replacerMask = 0;
            //combine 
            if((newForceVals & (int)EGridTileSettings.BlockerExtraSmall) != 0)
            {
                replacerMask |= (int)EGridTileSettings.BlockerExtraSmall;
            }
            if ((newForceVals & (int)EGridTileSettings.BlockerSmall) != 0)
            {
                replacerMask |= (int)EGridTileSettings.BlockerSmall;
            }
            if ((newForceVals & (int)EGridTileSettings.BlockerMedium) != 0)
            {
                replacerMask |= (int)EGridTileSettings.BlockerMedium;
            }
            if ((newForceVals & (int)EGridTileSettings.BlockerLarge) != 0)
            {
                replacerMask |= (int)EGridTileSettings.BlockerLarge;
            }
            if ((newForceVals & (int)EGridTileSettings.BlockerExtraLarge) != 0)
            {
                replacerMask |= (int)EGridTileSettings.BlockerExtraLarge;
            }

            return temp & ~replacerMask | newForceVals;
        }

    }
}
