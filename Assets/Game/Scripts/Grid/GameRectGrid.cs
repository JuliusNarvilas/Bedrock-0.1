
using Common.Grid;
using Common.Grid.Specializations;

namespace Game.Grid
{
    public class GameRectGrid : CuboidGrid<GameGridTileData, GameGridChangeContext>
    {
        public GameRectGrid(int i_SizeX, int i_SizeY, bool i_AllowMoveDiagonally)
        {
            m_SizeX = i_SizeX;
            m_SizeY = i_SizeY;
            m_AllowMoveDiagonally = i_AllowMoveDiagonally;

            for(int i = 0; i < m_SizeX; ++i)
            {
                for(int j = 0; j < m_SizeY; ++j)
                {
                    m_Tiles.Add(new GameGridTile(new GridPosition3D(i, j, 0)));
                }
            }
        }
    }
}
