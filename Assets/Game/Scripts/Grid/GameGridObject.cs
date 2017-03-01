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

    }

    public class GameGridObject : GridObject<GridPosition3D, GameGridTileData, GameGridChangeContext>
    {
        public override void Damage(IGridControl<GridPosition3D, GameGridTileData, GameGridChangeContext> i_Grid, GridPosition3D i_Position, GameGridChangeContext i_Context)
        {
            throw new NotImplementedException();
        }
    }
}
