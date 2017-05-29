using System;
using System.Collections.Generic;
using Common.Grid.Path;
using Common.Grid.Path.Specializations;

namespace Common.Grid.Specializations
{
    public class RectangleGrid<TTileData, TContext> : IGridControl<GridPosition2D, TTileData, TContext>
    {
        protected readonly List<GridTile<GridPosition2D, TTileData, TContext>> m_Tiles = new List<GridTile<GridPosition2D, TTileData, TContext>>();

        protected int m_SizeX;
        protected int m_SizeY;
        protected bool m_AllowMoveDiagonally = true;
        protected float m_NonDiagonalHeuristicFactor = 1.0f;
        protected float m_DiagonalHeuristicFactor = (float)Math.Sqrt(2.0);

        public int GetSizeX()
        {
            return m_SizeX;
        }
        public int GetSizeY()
        {
            return m_SizeY;
        }

        public int GetHeuristicDistance(GridPosition2D i_From, GridPosition2D i_To)
        {
            //http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
            int xDiff = Math.Abs(i_To.X - i_From.X);
            int yDiff = Math.Abs(i_To.Y - i_From.Y);
            
            if (m_AllowMoveDiagonally)
            {
                return (int)(m_NonDiagonalHeuristicFactor * (xDiff + yDiff) + (m_DiagonalHeuristicFactor - 2.0f * m_NonDiagonalHeuristicFactor) * Math.Min(xDiff, yDiff) + 0.5f);
            }
            else
            {
                return (int)(m_NonDiagonalHeuristicFactor * (xDiff + yDiff) + 0.5f);
            }
        }
        
        public bool TryGetTile(GridPosition2D i_Position, out GridTile<GridPosition2D, TTileData, TContext> o_Tile)
        {
            if (i_Position.X >= 0 && i_Position.X < m_SizeX && i_Position.Y >= 0 && i_Position.Y < m_SizeY)
            {
                int tilesIndex = i_Position.X * m_SizeY + i_Position.Y;
                o_Tile = m_Tiles[tilesIndex];
                return true;
            }
            o_Tile = null;
            return false;
        }


        public void GetConnected(GridPosition2D i_Position, List<GridTile<GridPosition2D, TTileData, TContext>> o_ConnectedTiles)
        {
            int startX = Math.Max(i_Position.X - 1, 0);
            int startY = Math.Max(i_Position.Y - 1, 0);
            int endX = Math.Min(i_Position.X + 2, m_SizeX);
            int endY = Math.Min(i_Position.Y + 2, m_SizeY);

            for(int itX = startX; itX < endX; ++itX)
            {
                for(int itY = startY; itY < endY; ++itY)
                {
                    if (m_AllowMoveDiagonally)
                    {
                        int tileIndex = itX * m_SizeY + itY;
                        o_ConnectedTiles.Add(m_Tiles[tileIndex]);
                    }
                    else if((itX != startX && itX != endX) || (itY != startY && itY != endY))
                    {
                        int tileIndex = itX * m_SizeY + itY;
                        o_ConnectedTiles.Add(m_Tiles[tileIndex]);
                    }
                }
            }
        }

        public GridPath<GridPosition2D, TTileData, TContext> GetPath(GridPosition2D i_Start, GridPosition2D i_End, TContext i_Context)
        {
            const int pathingDataMargin = 4;
            var pathData = GridPathData2DProvider<TTileData, TContext>.GLOBAL.GetGridPathData();

            int minX = Math.Max(i_Start.X - pathingDataMargin, 0);
            int minY = Math.Max(i_Start.Y - pathingDataMargin, 0);
            int maxX = Math.Min(i_End.X - pathingDataMargin, m_SizeX - 1);
            int maxY = Math.Min(i_End.Y - pathingDataMargin, m_SizeY - 1);

            pathData.Set(this, new GridPosition2D(minX, minY), new GridPosition2D(maxX, maxY));
            return new GridPath<GridPosition2D, TTileData, TContext>(this, pathData, i_Start, i_End, i_Context);
        }

        public GridPathArea<GridPosition2D, TTileData, TContext> GetPathArea(GridPosition2D i_Min, GridPosition2D i_Max, GridPosition2D i_Origin, TContext i_Context)
        {
            var pathData = GridPathData2DProvider<TTileData, TContext>.GLOBAL.GetGridPathData();
            pathData.Set(this, i_Min, i_Max);
            return new GridPathArea<GridPosition2D, TTileData, TContext>(this, pathData, i_Min, i_Max, i_Origin, i_Context);
        }
    }
}
