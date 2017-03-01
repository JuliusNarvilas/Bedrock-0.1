
namespace Common.Grid.Path.Specializations
{
    public class GridPathData3DProvider<TTileData, TContext> : GridPathDataProvider<GridPosition3D, TTileData, TContext>
    {
        public static readonly GridPathData3DProvider<TTileData, TContext> GLOBAL = new GridPathData3DProvider<TTileData, TContext>();

        protected override IGridPathData<GridPosition3D, TTileData, TContext> Create()
        {
            return new GridPathData3D<TTileData, TContext>(this);
        }
    }
}
