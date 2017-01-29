using Common.Grid;

namespace Game.Grid
{
    public class GameGridTile : GridTile<GameGridTerrain, GridPosition2D, GameGridChangeContext>
    {
        private bool m_IsBlocked = false;

        public GameGridTile(GridPosition2D i_Position, GameGridTerrain i_Terrain) : base(i_Position, i_Terrain)
        {
        }

        public override float GetTransitionInCost(GridTile<GameGridTerrain, GridPosition2D, GameGridChangeContext> i_FromTile, GameGridChangeContext i_Context)
        {
            return m_IsBlocked ? -1 : Terrain.GetCost(i_Context);
        }

        //*area state effect (fire / electricity / ***)
    }
}
