
using Common.Grid;

namespace Game.Grid
{
    public class GameGridChangeContext :
        BasicGridContext<GameGridTile, GameGridTerrain, GridPosition2D, GameGridChangeContext>
    {
        public int temp;
    }
}
