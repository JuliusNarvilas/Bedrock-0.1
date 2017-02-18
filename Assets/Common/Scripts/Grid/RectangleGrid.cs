﻿using System;
using System.Collections.Generic;
using Common.Grid.Path;

namespace Common.Grid
{
    public class RectangleGrid<TTile, TTerrain, TContext> : IGridControl<TTile, TTerrain, GridPosition2D, TContext>
        where TTile : GridTile<TTerrain, GridPosition2D, TContext>
        where TTerrain : GridTerrain<TContext>
        where TContext : IGridContext<TTile, TTerrain, GridPosition2D, TContext>
    {
        protected readonly List<TTile> m_Tiles = new List<TTile>();

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
        
        public bool TryGetTile(GridPosition2D i_Position, out TTile o_Tile)
        {
            if (i_Position.X >= 0 && i_Position.X < m_SizeX && i_Position.Y >= 0 && i_Position.Y < m_SizeY)
            {
                int tilesIndex = i_Position.Y * m_SizeX + i_Position.X;
                o_Tile = m_Tiles[tilesIndex];
                return true;
            }
            o_Tile = null;
            return false;
        }


        public void GetConnected(GridPosition2D i_Position, List<TTile> o_ConnectedTiles)
        {
            //left side
            TTile tempElement = null;
            if(TryGetTile(new GridPosition2D(i_Position.X - 1, i_Position.Y), out tempElement))
            {
                o_ConnectedTiles.Add(tempElement);

                if (m_AllowMoveDiagonally)
                {
                    if (TryGetTile(new GridPosition2D(i_Position.X - 1, i_Position.Y - 1), out tempElement))
                    {
                        o_ConnectedTiles.Add(tempElement);
                    }
                    if (TryGetTile(new GridPosition2D(i_Position.X - 1, i_Position.Y + 1), out tempElement))
                    {
                        o_ConnectedTiles.Add(tempElement);
                    }
                }
            }
            //right side
            if (TryGetTile(new GridPosition2D(i_Position.X + 1, i_Position.Y), out tempElement))
            {
                o_ConnectedTiles.Add(tempElement);

                if (m_AllowMoveDiagonally)
                {
                    if (TryGetTile(new GridPosition2D(i_Position.X + 1, i_Position.Y - 1), out tempElement))
                    {
                        o_ConnectedTiles.Add(tempElement);
                    }
                    if (TryGetTile(new GridPosition2D(i_Position.X + 1, i_Position.Y + 1), out tempElement))
                    {
                        o_ConnectedTiles.Add(tempElement);
                    }
                }
            }

            if (TryGetTile(new GridPosition2D(i_Position.X, i_Position.Y - 1), out tempElement))
            {
                o_ConnectedTiles.Add(tempElement);
            }
            if (TryGetTile(new GridPosition2D(i_Position.X, i_Position.Y + 1), out tempElement))
            {
                o_ConnectedTiles.Add(tempElement);
            }
        }

        public GridPath<TTile, TTerrain, GridPosition2D, TContext> GetPath(GridPosition2D i_Start, GridPosition2D i_End, TContext i_Context)
        {
            const int pathingDataMargin = 4;
            var pathData = GridPathData2DProvider<TTile, TTerrain, TContext>.GLOBAL.GetGridPathData();

            int minX = Math.Max(i_Start.X - pathingDataMargin, 0);
            int minY = Math.Max(i_Start.Y - pathingDataMargin, 0);
            int maxX = Math.Min(i_End.X - pathingDataMargin, m_SizeX - 1);
            int maxY = Math.Min(i_End.Y - pathingDataMargin, m_SizeY - 1);

            pathData.Set(this, new GridPosition2D(minX, minY), new GridPosition2D(maxX, maxY));
            return new GridPath<TTile, TTerrain, GridPosition2D, TContext>(this, pathData, i_Start, i_End, i_Context);
        }

        public GridArea<TTile, TTerrain, GridPosition2D, TContext> GetPathArea(GridPosition2D i_Min, GridPosition2D i_Max, GridPosition2D i_Origin, TContext i_Context)
        {
            var pathData = GridPathData2DProvider<TTile, TTerrain, TContext>.GLOBAL.GetGridPathData();
            pathData.Set(this, i_Min, i_Max);
            return new GridArea<TTile, TTerrain, GridPosition2D, TContext>(this, pathData, i_Min, i_Max, i_Origin, i_Context);
        }
    }
}
