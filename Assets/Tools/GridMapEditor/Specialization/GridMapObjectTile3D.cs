using Common;
using Common.Grid;
using Game.Grid;
using System;
using UnityEngine;

namespace Tools.Specialization
{
    [Serializable]
    public class GridMapObjectTile3D : IGridMapObjectTile<GridPosition3D, int>
    {
        [SerializeField]
        private GridPosition3D m_Position;

        [SerializeField]
        [EnumFlag(typeof(GridMapObjectTile3D), "ConvertBlockerEnumToFlags", "ConvertBlockerEnumToValue")]
        private EGridTileSettings m_TileBlockerSettings;

        [SerializeField]
        [EnumFlag]
        private EGameGridObjectSettings m_Settings;

        public GridPosition3D Position
        {
            get { return m_Position; }
        }

        public int Settings
        {
            get { return (int) m_Settings | (int) m_TileBlockerSettings; }
        }



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
