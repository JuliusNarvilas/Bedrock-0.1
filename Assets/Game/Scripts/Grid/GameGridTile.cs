using System;
using Common.Grid;
using Common.Grid.Path;

namespace Game.Grid
{
    public class GameGridTileData : IGridTileData<GridPosition3D, GameGridTileData, GameGridChangeContext>
    {
        public float GetCost(IGridControl<GridPosition3D, GameGridTileData, GameGridChangeContext> i_Grid, GridPathElement<GridPosition3D, GameGridTileData, GameGridChangeContext> i_Source, GameGridChangeContext i_Context)
        {
            throw new NotImplementedException();
        }

        public float GetDangerFactor()
        {
            throw new NotImplementedException();
        }
    }

    public class GameGridTile : GridTile<GridPosition3D, int, GameGridChangeContext>
    {
        private bool m_IsBlocked = false;

        public GameGridTile(GridPosition3D i_Position) : base(i_Position, 0)
        {
        }

        public override float GetCost(IGridControl<GridPosition3D, int, GameGridChangeContext> i_Grid, GridPathElement<GridPosition3D, int, GameGridChangeContext> i_Source, GameGridChangeContext i_Context)
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
