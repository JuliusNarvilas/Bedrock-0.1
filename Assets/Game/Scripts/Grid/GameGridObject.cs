using Common;
using Common.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Grid
{
    [Flags]
    public enum GameGridObjectSettings : int
    {
        None = 0,
        Setting1 = GridTileBlockerFlags.CustomFlagStart,
        Setting2 = Setting1 << 1,
        Setting3 = Setting1 << 2,
        Setting4 = Setting1 | Setting2
    }

    [Serializable]
    public class GameGridObject : GridObject<GridPosition3D, GameGridTileData, GameGridChangeContext>
    {
        public override void Damage(IGridControl<GridPosition3D, GameGridTileData, GameGridChangeContext> i_Grid, GridPosition3D i_Position, GameGridChangeContext i_Context)
        {
            throw new NotImplementedException();
        }
    }
}
