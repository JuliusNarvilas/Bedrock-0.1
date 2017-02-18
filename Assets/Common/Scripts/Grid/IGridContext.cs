using Common.Grid.Path;

namespace Common.Grid
{
    public interface IGridContext<TTile, TTerrain, TPosition, TFinalContext>
        where TTile : GridTile<TTerrain, TPosition, TFinalContext>
        where TTerrain : GridTerrain<TFinalContext>
        where TFinalContext : IGridContext<TTile, TTerrain, TPosition, TFinalContext>
    {
        float GetCost(
            IGridControl<TTile, TTerrain, TPosition, TFinalContext> i_Grid,
            GridPathElement<TTile, TTerrain, TPosition, TFinalContext> i_Destination,
            GridPathElement<TTile, TTerrain, TPosition, TFinalContext> i_Source
            );
    }

    public class BasicGridContext<TTile, TTerrain, TPosition, TFinalContext> : IGridContext<TTile, TTerrain, TPosition, TFinalContext>
        where TTile : GridTile<TTerrain, TPosition, TFinalContext>
        where TTerrain : GridTerrain<TFinalContext>
        where TFinalContext : BasicGridContext<TTile, TTerrain, TPosition, TFinalContext>
    {
        public float GetCost(
            IGridControl<TTile, TTerrain, TPosition, TFinalContext> i_Grid,
            GridPathElement<TTile, TTerrain, TPosition, TFinalContext> i_Destination,
            GridPathElement<TTile, TTerrain, TPosition, TFinalContext> i_Source
        )
        {
            TFinalContext original = (TFinalContext)this;
            float terrainCost = i_Source.Tile.GetTransitionOutCost(i_Destination.Tile, original);
            terrainCost += i_Destination.Tile.GetTransitionInCost(i_Source.Tile, original);
            return terrainCost;
        }
    }
}
