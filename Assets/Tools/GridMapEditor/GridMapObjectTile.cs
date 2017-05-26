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
        public GridTileBlockerFlags TileBlockerSettings;
        [EnumFlag]
        public GameGridObjectSettings Settings;

        private static int ConvertBlockerEnumToFlags(int i_Value)
        {
            return GridHelpers.BlockerValueToIndexFlags(i_Value);
        }
        private static int ConvertBlockerEnumToValue(int i_Value, int i_NewValue)
        {
            var temp = GridHelpers.BlockerIndexFlagsToValue(i_Value);
            var newForceVals = GridHelpers.BlockerIndexFlagsToValue(i_NewValue);
            int replacerMask = 0;
            if((newForceVals & (int)GridTileBlockerFlags.LeftAnyBlocker) != 0)
            {
                replacerMask |= (int)GridTileBlockerFlags.LeftAnyBlocker;
            }
            if ((newForceVals & (int)GridTileBlockerFlags.RightAnyBlocker) != 0)
            {
                replacerMask |= (int)GridTileBlockerFlags.RightAnyBlocker;
            }
            if ((newForceVals & (int)GridTileBlockerFlags.ForwardAnyBlocker) != 0)
            {
                replacerMask |= (int)GridTileBlockerFlags.ForwardAnyBlocker;
            }
            if ((newForceVals & (int)GridTileBlockerFlags.BackwardAnyBlocker) != 0)
            {
                replacerMask |= (int)GridTileBlockerFlags.BackwardAnyBlocker;
            }

            return temp & ~replacerMask | newForceVals;
        }

    }
}
