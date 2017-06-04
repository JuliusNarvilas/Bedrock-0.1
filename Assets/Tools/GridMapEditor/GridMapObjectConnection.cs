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
        [EnumFlag(typeof(GridMapObjectConnection), "ConvertConnectionSettingsEnumToFlags", "ConvertConnectionSettingsEnumToValue")]
        public EConnectionSettings Settings;
        

        private static int ConvertConnectionSettingsEnumToFlags(int i_Value)
        {
            return i_Value >> 3;
        }

        private static int ConvertConnectionSettingsEnumToValue(int i_Value, int i_NewFags)
        {
            return i_Value << 3;
        }
    }
}
