
using Common.Grid.Path;

namespace Common.Grid
{
    /// <summary>
    /// Representation of grid tile data and functionality.
    /// </summary>
    /// <typeparam name="TPosition">Type of the position.</typeparam>
    /// <typeparam name="TContext">Type of the context data for function calls.</typeparam>
    public abstract class GridTile<TPosition, TContext, TOwnType> where TOwnType : GridTile<TPosition, TContext, TOwnType>
    {
        public readonly TPosition Position;
        /// <summary>
        /// A modifier to pathing heuristics to make this tile seem more expensive to move through.
        /// </summary>
        public float PathingDangerFactor;

        public int Settings;
        
        public GridTile(TPosition i_Position)
        {
            Position = i_Position;
        }

        public override string ToString()
        {
            return string.Format(
                "GridTile: { Position: {0} }",
                Position.ToString()
                );
        }

        /// <summary>
        /// Cost calculation to move into this tile from <paramref name="i_Source"/> tile.
        /// Used mostly in pathing calculations.
        /// </summary>
        /// <param name="i_Grid">Grid of this tile.</param>
        /// <param name="i_Source">Source of the transition cost to calculate.</param>
        /// <param name="i_Context">Contextual information about this function call.</param>
        /// <returns></returns>
        public abstract float GetCost(IGridControl<TPosition, TContext, TOwnType> i_Grid, GridPathElement<TPosition, TContext, TOwnType> i_Source, TContext i_Context);
    }
}
