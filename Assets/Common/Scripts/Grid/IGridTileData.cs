
using Common.Grid.Path;

namespace Common.Grid
{
    public interface IGridTileData<TPosition, TTileData, TContext>
    {
        float GetDangerFactor();

        float GetCost(
                IGridControl<TPosition, TTileData, TContext> i_Grid,
                GridPathElement<TPosition, TTileData, TContext> i_Source,
                TContext i_Context
                );

        /*
        public interface IGridContext<TPosition, TTile, TTileData, TContext>
        where TTile : GridTile<TPosition, TTileData, TContext>
        where TFinalContext : IGridContext<TPosition, TTile, TTileData, TFinalContext>
        {
            
        }
        public float GetCost(
            IGridControl<TTile, TPosition, TTileData, TFinalContext> i_Grid,
            GridPathElement<TTile, TPosition, TTileData, TFinalContext> i_Destination,
            GridPathElement<TTile, TPosition, TTileData, TFinalContext> i_Source
        )
        {
            TFinalContext original = (TFinalContext)this;
            float terrainCost = i_Source.Tile.GetTransitionOutCost(i_Destination.Tile, original);
            terrainCost += i_Destination.Tile.GetTransitionInCost(i_Source.Tile, original);
            return terrainCost;
        }
        */
    }
}
