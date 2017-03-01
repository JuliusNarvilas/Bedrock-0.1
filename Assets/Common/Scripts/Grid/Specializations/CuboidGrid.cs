using System;
using System.Collections.Generic;
using Common.Grid.Path;
using Common.Grid.Path.Specializations;

namespace Common.Grid.Specializations
{
    public class CuboidGrid<TTileData, TContext> : IGridControl<GridPosition3D, TTileData, TContext>
    {
        protected readonly List<GridTile<GridPosition3D, TTileData, TContext>> m_Tiles = new List<GridTile<GridPosition3D, TTileData, TContext>>();

        protected int m_SizeX;
        protected int m_SizeY;
        protected int m_SizeZ;
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
        public int GetSizeZ()
        {
            return m_SizeZ;
        }

        public int GetHeuristicDistance(GridPosition3D i_From, GridPosition3D i_To)
        {
            int xDiff = Math.Abs(i_To.X - i_From.X);
            int yDiff = Math.Abs(i_To.Y - i_From.Y);

            if (m_AllowMoveDiagonally)
            {
                return (int)(m_NonDiagonalHeuristicFactor * (xDiff + yDiff) + (m_DiagonalHeuristicFactor - 2.0f * m_NonDiagonalHeuristicFactor) * Math.Min(xDiff, yDiff) + 0.5f);
            }
            else
            {
                return xDiff + yDiff;
            }
        }

        public bool TryGetTile(GridPosition3D i_Position, out GridTile<GridPosition3D, TTileData, TContext> o_Tile)
        {
            if (i_Position.X >= 0 && i_Position.X < m_SizeX && i_Position.Y >= 0 && i_Position.Y < m_SizeY && i_Position.Z >= 0 && i_Position.Z < m_SizeZ)
            {
                int tilesIndex = i_Position.X * m_SizeY + i_Position.Y;
                o_Tile = m_Tiles[tilesIndex];
                return true;
            }
            o_Tile = null;
            return false;
        }


        public void GetConnected(GridPosition3D i_Position, List<GridTile<GridPosition3D, TTileData, TContext>> o_ConnectedTiles)
        {
            //TODO: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //left side
            GridTile<GridPosition3D, TTileData, TContext> tempElement = null;
            if (TryGetTile(new GridPosition3D(i_Position.X - 1, i_Position.Y), out tempElement))
            {
                o_ConnectedTiles.Add(tempElement);

                if (m_AllowMoveDiagonally)
                {
                    if (TryGetTile(new GridPosition3D(i_Position.X - 1, i_Position.Y - 1), out tempElement))
                    {
                        o_ConnectedTiles.Add(tempElement);
                    }
                    if (TryGetTile(new GridPosition3D(i_Position.X - 1, i_Position.Y + 1), out tempElement))
                    {
                        o_ConnectedTiles.Add(tempElement);
                    }
                }
            }
            //right side
            if (TryGetTile(new GridPosition3D(i_Position.X + 1, i_Position.Y), out tempElement))
            {
                o_ConnectedTiles.Add(tempElement);

                if (m_AllowMoveDiagonally)
                {
                    if (TryGetTile(new GridPosition3D(i_Position.X + 1, i_Position.Y - 1), out tempElement))
                    {
                        o_ConnectedTiles.Add(tempElement);
                    }
                    if (TryGetTile(new GridPosition3D(i_Position.X + 1, i_Position.Y + 1), out tempElement))
                    {
                        o_ConnectedTiles.Add(tempElement);
                    }
                }
            }

            if (TryGetTile(new GridPosition3D(i_Position.X, i_Position.Y - 1), out tempElement))
            {
                o_ConnectedTiles.Add(tempElement);
            }
            if (TryGetTile(new GridPosition3D(i_Position.X, i_Position.Y + 1), out tempElement))
            {
                o_ConnectedTiles.Add(tempElement);
            }
        }

        public GridPath<GridPosition3D, TTileData, TContext> GetPath(GridPosition3D i_Start, GridPosition3D i_End, TContext i_Context)
        {
            const int pathingDataMargin = 4;
            var pathData = GridPathData3DProvider<TTileData, TContext>.GLOBAL.GetGridPathData();

            int minX = Math.Max(i_Start.X - pathingDataMargin, 0);
            int minY = Math.Max(i_Start.Y - pathingDataMargin, 0);
            int maxX = Math.Min(i_End.X - pathingDataMargin, m_SizeX - 1);
            int maxY = Math.Min(i_End.Y - pathingDataMargin, m_SizeY - 1);

            pathData.Set(this, new GridPosition3D(minX, minY), new GridPosition3D(maxX, maxY));
            return new GridPath<GridPosition3D, TTileData, TContext>(this, pathData, i_Start, i_End, i_Context);
        }

        public GridArea<GridPosition3D, TTileData, TContext> GetPathArea(GridPosition3D i_Min, GridPosition3D i_Max, GridPosition3D i_Origin, TContext i_Context)
        {
            var pathData = GridPathData3DProvider<TTileData, TContext>.GLOBAL.GetGridPathData();
            pathData.Set(this, i_Min, i_Max);
            return new GridArea<GridPosition3D, TTileData, TContext>(this, pathData, i_Min, i_Max, i_Origin, i_Context);
        }
    }
}
