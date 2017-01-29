using System;
using System.Collections.Generic;
using Common.Grid.Path;

namespace Common.Grid
{
    public class RectangleGrid<TTile, TTerrain, TContext> : IGridControl<TTile, TTerrain, GridPosition2D, TContext>
        where TTile : GridTile<TTerrain, GridPosition2D, TContext>
        where TTerrain : GridTerrain<TContext>
    {
        protected readonly List<TTile> m_Tiles = new List<TTile>();
        protected readonly List<List<GridPathElement<TTile, TTerrain, GridPosition2D, TContext>>> m_PathDataList = new List<List<GridPathElement<TTile, TTerrain, GridPosition2D, TContext>>>();

        protected int m_SizeX;
        protected int m_SizeY;
        protected bool m_AllowMoveDiagonally = true;

        public int GetHeuristicDistance(GridPosition2D i_From, GridPosition2D i_To)
        {
            int xDiff = Math.Abs(i_To.X - i_From.X);
            int yDiff = Math.Abs(i_To.Y - i_From.Y);

            if (m_AllowMoveDiagonally)
            {
                int minDiff = Math.Min(xDiff, yDiff);
                int diagonalDist = (int)(Math.Sqrt(minDiff * minDiff * 2) + 0.5);
                return diagonalDist + xDiff - minDiff + yDiff - minDiff;
            }
            else
            {
                return xDiff + yDiff;
            }
        }
        
        public TTile GetTile(GridPosition2D i_Position)
        {
            int tilesIndex = i_Position.Y * m_SizeX + i_Position.X;
            Log.DebugAssert(
                i_Position.X >= 0 && i_Position.Y >= 0 &&
                tilesIndex <= m_SizeX * m_SizeY,
                "RectangleGrid:GetTile position out of bounds ({0}, max {1};{2})", i_Position, m_SizeX, m_SizeY
            );
            return m_Tiles[tilesIndex];
        }
        
        public bool TryGetTile(GridPosition2D i_Position, out TTile o_Tile)
        {
            int tilesIndex = i_Position.Y * m_SizeX + i_Position.X;
            if (i_Position.X >= 0 && i_Position.Y >= 0 && tilesIndex <= m_SizeX * m_SizeY)
            {
                o_Tile = m_Tiles[tilesIndex];
                return true;
            }
            o_Tile = null;
            return false;
        }

        //TODO: IMPROVE and make it more readable
        public void GetConnected(GridPosition2D i_Position, List<TTile> o_ConnectedElements)
        {
            TTile tempElement = null;
            if (i_Position.X > 0)
            {
                tempElement = GetTile(new GridPosition2D(i_Position.X - 1, i_Position.Y));
                if (tempElement != null) o_ConnectedElements.Add(tempElement);
                if (m_AllowMoveDiagonally)
                {
                    tempElement = GetTile(new GridPosition2D(i_Position.X - 1, i_Position.Y - 1));
                    if (tempElement != null) o_ConnectedElements.Add(tempElement);
                    tempElement = GetTile(new GridPosition2D(i_Position.X - 1, i_Position.Y + 1));
                    if (tempElement != null) o_ConnectedElements.Add(tempElement);
                }
            }
            tempElement = GetTile(new GridPosition2D(i_Position.X + 1, i_Position.Y));
            if (tempElement != null) o_ConnectedElements.Add(tempElement);
            if (m_AllowMoveDiagonally)
            {
                tempElement = GetTile(new GridPosition2D(i_Position.X + 1, i_Position.Y - 1));
                if (tempElement != null) o_ConnectedElements.Add(tempElement);
                tempElement = GetTile(new GridPosition2D(i_Position.X + 1, i_Position.Y + 1));
                if (tempElement != null) o_ConnectedElements.Add(tempElement);
            }

            if (i_Position.Y > 0)
            {
                tempElement = GetTile(new GridPosition2D(i_Position.X, i_Position.Y - 1));
                if (tempElement != null)
                {
                    o_ConnectedElements.Add(tempElement);
                }
            }
            tempElement = GetTile(new GridPosition2D(i_Position.X, i_Position.Y + 1));
            if (tempElement != null)
            {
                o_ConnectedElements.Add(tempElement);
            }
        }

        public GridPath<TTile, TTerrain, GridPosition2D, TContext> GetPath(GridPosition2D i_Start, GridPosition2D i_End, TContext i_Context)
        {
            return new GridPath<TTile, TTerrain, GridPosition2D, TContext>(this, null, i_Start, i_End, i_Context);
        }

        public void GetPathArea(GridPosition2D i_Min, GridPosition2D i_Max, TContext i_Context)
        {
            throw new NotImplementedException();
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
