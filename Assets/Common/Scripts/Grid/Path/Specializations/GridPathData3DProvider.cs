
namespace Common.Grid.Path.Specializations
{
    public class GridPathData3DProvider<TContext, TTile> : GridPathDataProvider<GridPosition3D, TContext, TTile> where TTile : GridTile<GridPosition3D, TContext, TTile>
    {
        public static readonly GridPathData3DProvider<TContext, TTile> GLOBAL = new GridPathData3DProvider<TContext, TTile>();

        protected override IGridPathData<GridPosition3D, TContext, TTile> Create()
        {
            return new GridPathData3D<TContext, TTile>(this);
        }
    }
}
