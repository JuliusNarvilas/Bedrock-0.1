using System;
using Common.Grid;
using Common.Grid.Path;

namespace Game.Grid
{
    public class GameGridTileData
    {
    }

    public class GameGridTile : GridTile<GridPosition3D, GameGridTileData, GameGridChangeContext>
    {

        public GameGridTile(GridPosition3D i_Position) : base(i_Position)
        {
        }

        public override float GetCost(IGridControl<GridPosition3D, GameGridTileData, GameGridChangeContext> i_Grid, GridPathElement<GridPosition3D, GameGridTileData, GameGridChangeContext> i_Source, GameGridChangeContext i_Context)
        {
            throw new NotImplementedException();
        }

        public override float GetDangerFactor()
        {
            throw new NotImplementedException();
        }

        //*area state effect (fire / electricity / ***)
    }
}
