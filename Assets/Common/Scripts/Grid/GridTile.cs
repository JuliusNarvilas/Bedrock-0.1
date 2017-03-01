
using Common.Grid.Path;

namespace Common.Grid
{
    /// <summary>
    /// Representation of grid tile data and functionality.
    /// </summary>
    /// <typeparam name="TPosition">The type of the position.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public abstract class GridTile<TPosition, TTileData, TContext> : IGridTileData<TPosition, TTileData, TContext>
    {
        public readonly TPosition Position;
        public TTileData Data;
        
        public GridTile(TPosition i_Position, TTileData i_Data = default(TTileData))
        {
            Position = i_Position;
            Data = i_Data;
        }

        public override string ToString()
        {
            return string.Format(
                "GridTile: { Position: {0} ; Data: {2} }",
                Position.ToString(),
                Data.ToString()
                );
        }
        
        public abstract float GetDangerFactor();
        public abstract float GetCost(IGridControl<TPosition, TTileData, TContext> i_Grid, GridPathElement<TPosition, TTileData, TContext> i_Source, TContext i_Context);
    }
}
