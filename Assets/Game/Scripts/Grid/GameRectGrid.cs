
using Common.Grid;
using Common.Grid.Specializations;
using UnityEngine;

namespace Game.Grid
{
    public class GameRectGrid : CuboidGrid<GameGridChangeContext, GameGridTile>
    {
        public GameRectGrid(int i_SizeX, int i_SizeY, int I_SizeZ, bool i_AllowMoveDiagonally): base(i_SizeX, i_SizeY, I_SizeZ, new Vector3(1, 2, 1))
        {
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
