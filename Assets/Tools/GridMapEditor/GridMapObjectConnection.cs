using Common;
using Common.Grid;
using Common.Grid.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    [Serializable]
    public class GridMapObjectConnection
    {
        public GridPosition3D Position;
        public GridTileLocation TileLocation;
        [EnumFlag(typeof(GridMapObjectConnection), "ConvertConnectionSettingsEnum")]
        public ConnectionSettings Settings;
        

        private static int ConvertConnectionSettingsEnum(int i_Value, bool i_Export)
        {
            if (i_Export)
            {
                return i_Value >> 3;
            }
            return i_Value << 3;
        }
    }
}
