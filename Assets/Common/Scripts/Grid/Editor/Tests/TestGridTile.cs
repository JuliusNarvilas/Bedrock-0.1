using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Grid.Path;

namespace Common.Grid.Editor
{
    public class TestGridTile : GridTile<GridPosition3D, int, TestGridTile>
    {
        float Cost;

        public TestGridTile(GridPosition3D i_Position, float i_Cost) : base(i_Position)
        {
            Cost = i_Cost;
        }

        public override float GetCost(IGridControl<GridPosition3D, int, TestGridTile> i_Grid, GridPathElement<GridPosition3D, int, TestGridTile> i_Source, int i_Context)
        {
            return Cost;
        }
    }
}
