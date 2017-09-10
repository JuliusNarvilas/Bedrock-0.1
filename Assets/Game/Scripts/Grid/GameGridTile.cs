using System;
using Common.Grid;
using Common.Grid.Path;

namespace Game.Grid
{
    public class GameGridTile : GridTile<GridPosition3D, GameGridChangeContext, GameGridTile>
    {

        public GameGridTile(GridPosition3D i_Position) : base(i_Position)
        {
        }

        public override float GetCost(IGridControl<GridPosition3D, GameGridChangeContext, GameGridTile> i_Grid, GridPathElement<GridPosition3D, GameGridChangeContext, GameGridTile> i_Source, GameGridChangeContext i_Context, out int i_AvoidanceLevel)
        {
            throw new NotImplementedException();
        }

        //*area state effect (fire / electricity / ***)
    }
}
