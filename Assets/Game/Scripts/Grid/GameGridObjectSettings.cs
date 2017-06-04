using Common;
using Common.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Grid
{
    [Flags]
    public enum EGameGridObjectSettings : int
    {
        None = 0,
        Setting1 = EGridTileSettings.CustomSettingsStart,
        Setting2 = Setting1 << 1,
        Setting3 = Setting1 << 2,
        Setting4 = Setting1 | Setting2
    }


}
