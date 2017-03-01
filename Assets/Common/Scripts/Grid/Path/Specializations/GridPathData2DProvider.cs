
namespace Common.Grid.Path.Specializations
{
    public class GridPathData2DProvider<TTileData, TContext> : GridPathDataProvider<GridPosition2D, TTileData, TContext>
    {
        public static readonly GridPathData2DProvider<TTileData, TContext> GLOBAL = new GridPathData2DProvider<TTileData, TContext>();

        protected override IGridPathData<GridPosition2D, TTileData, TContext> Create()
        {
            return new GridPathData2D<TTileData, TContext>(this);
        }
    }
}
