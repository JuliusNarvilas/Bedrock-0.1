
namespace Common.Grid.Path.Specializations
{
    /// <summary>
    /// 
    /// </summary>
    public class GridPathData2DProvider<TContext, TTile> : GridPathDataProvider<GridPosition2D, TContext, TTile> where TTile : GridTile<GridPosition2D, TContext, TTile>
    {
        public static readonly GridPathData2DProvider<TContext, TTile> GLOBAL = new GridPathData2DProvider<TContext, TTile>();

        protected override IGridPathData<GridPosition2D, TContext, TTile> Create()
        {
            return new GridPathData2D<TContext, TTile>(this);
        }
    }
}
