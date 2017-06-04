using System;
using System.Collections.Generic;
using Common.Grid.Path;
using Common.Grid.Path.Specializations;
using Common.Grid.Physics;
using UnityEngine;
using Common.Grid.Physics.Specializations;

namespace Common.Grid.Specializations
{
    public class CuboidGrid<TContext, TTile> : IGridControl<GridPosition3D, TContext, TTile> where TTile : GridTile<GridPosition3D, TContext, TTile>
    {
        protected readonly List<TTile> m_Tiles = new List<TTile>();

        protected int m_SizeX;
        protected int m_SizeY;
        protected int m_SizeZ;
        protected bool m_AllowMoveDiagonally = true;
        protected float m_NonDiagonalHeuristicFactor = 1.0f;
        protected float m_DiagonalHeuristicFactor = (float)Math.Sqrt(2.0);
        protected float m_Diagonal3DHeuristicFactor = (float)Math.Sqrt(3.0);

        protected Vector3 m_TileSize;
        protected Vector3 m_GridOrigin;
        protected GridTilePhysicalShape m_TilePhysicalShape;

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

        public CuboidGrid()
        {
            m_TilePhysicalShape = new CuboidGridTilePhysicalShape(m_TileSize);
        }

        public int GetHeuristicDistance(GridPosition3D i_From, GridPosition3D i_To)
        {
            int xDiff = Math.Abs(i_To.X - i_From.X);
            int yDiff = Math.Abs(i_To.Y - i_From.Y);
            int zDiff = Math.Abs(i_To.Z - i_From.Z);

            if (m_AllowMoveDiagonally)
            {
                int smallestDiff = Math.Min(xDiff, yDiff);
                int smallDiff = xDiff + yDiff - smallestDiff;
                if (smallestDiff > zDiff)
                {
                    smallDiff = smallestDiff;
                    smallestDiff = zDiff;
                }
                else if (smallDiff > zDiff)
                {
                    smallDiff = zDiff;
                }

                return (int)(m_NonDiagonalHeuristicFactor * (xDiff + yDiff + zDiff) +
                    (m_Diagonal3DHeuristicFactor - 3.0f * m_NonDiagonalHeuristicFactor) * smallestDiff +
                    (m_DiagonalHeuristicFactor - 2.0f * m_NonDiagonalHeuristicFactor) * smallDiff
                    + 0.5f);
            }
            else
            {
                return (int)(m_NonDiagonalHeuristicFactor * (xDiff + yDiff + zDiff) + 0.5f);
            }
        }

        public bool TryGetTile(GridPosition3D i_Position, out TTile o_Tile)
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


        public void GetConnected(GridPosition3D i_Position, List<TTile> o_ConnectedTiles)
        {
            int startX = Math.Max(i_Position.X - 1, 0);
            int startY = Math.Max(i_Position.Y - 1, 0);
            int startZ = Math.Max(i_Position.Z - 1, 0);
            int endX = Math.Min(i_Position.X + 2, m_SizeX);
            int endY = Math.Min(i_Position.Y + 2, m_SizeY);
            int endZ = Math.Min(i_Position.Z + 2, m_SizeZ);
            int xStride = m_SizeY * m_SizeZ;

            for (int itX = startX; itX < endX; ++itX)
            {
                for (int itY = startY; itY < endY; ++itY)
                {
                    for (int itZ = startZ; itZ < endZ; ++itZ)
                    {
                        if (m_AllowMoveDiagonally)
                        {
                            int tileIndex = itX * xStride + itY * m_SizeZ + itZ;
                            o_ConnectedTiles.Add(m_Tiles[tileIndex]);
                        }
                        else if ((itX != startX && itX != endX) || (itY != startY && itY != endY) || (itZ != startZ && itZ != endZ))
                        {
                            int tileIndex = itX * xStride + itY * m_SizeZ + itZ;
                            o_ConnectedTiles.Add(m_Tiles[tileIndex]);
                        }
                    }
                }
            }
        }

        public GridPath<GridPosition3D, TContext, TTile> GetPath(GridPosition3D i_Start, GridPosition3D i_End, TContext i_Context)
        {
            const int pathingDataMargin = 4;
            var pathData = GridPathData3DProvider<TContext, TTile>.GLOBAL.GetGridPathData();

            int minX = Math.Max(i_Start.X - pathingDataMargin, 0);
            int minY = Math.Max(i_Start.Y - pathingDataMargin, 0);
            int maxX = Math.Min(i_End.X - pathingDataMargin, m_SizeX - 1);
            int maxY = Math.Min(i_End.Y - pathingDataMargin, m_SizeY - 1);

            pathData.Set(this, new GridPosition3D(minX, minY), new GridPosition3D(maxX, maxY));
            return new GridPath<GridPosition3D, TContext, TTile>(this, pathData, i_Start, i_End, i_Context);
        }

        public GridPathArea<GridPosition3D, TContext, TTile> GetPathArea(GridPosition3D i_Min, GridPosition3D i_Max, GridPosition3D i_Origin, TContext i_Context)
        {
            var pathData = GridPathData3DProvider<TContext, TTile>.GLOBAL.GetGridPathData();
            pathData.Set(this, i_Min, i_Max);
            return new GridPathArea<GridPosition3D, TContext, TTile>(this, pathData, i_Min, i_Max, i_Origin, i_Context);
        }

        public bool TryGetTilePhysicalData(GridPosition3D i_Position, out GridTilePhysicalData o_Tile)
        {
            Vector3 worldPos = new Vector3();
            worldPos.x = m_GridOrigin.x + i_Position.X * m_TileSize.x;
            worldPos.y = m_GridOrigin.y + i_Position.Y * m_TileSize.y;
            worldPos.z = m_GridOrigin.z + i_Position.Z * m_TileSize.z;
            o_Tile = new GridTilePhysicalData(worldPos, m_TilePhysicalShape);
            return true;
        }
    }
}
