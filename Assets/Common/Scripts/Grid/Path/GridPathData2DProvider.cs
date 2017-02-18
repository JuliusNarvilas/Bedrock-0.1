
namespace Common.Grid.Path
{
    public class GridPathData2DProvider<TTile, TTerrain, TContext> : GridPathDataProvider<TTile, TTerrain, GridPosition2D, TContext>
        where TTile : GridTile<TTerrain, GridPosition2D, TContext>
        where TTerrain : GridTerrain<TContext>
        where TContext : IGridContext<TTile, TTerrain, GridPosition2D, TContext>
    {
        public static readonly GridPathData2DProvider<TTile, TTerrain, TContext> GLOBAL = new GridPathData2DProvider<TTile, TTerrain, TContext>();

        protected override IGridPathData<TTile, TTerrain, GridPosition2D, TContext> Create()
        {
            return new GridPathData2D<TTile, TTerrain, TContext>(this);
        }
    }
}
