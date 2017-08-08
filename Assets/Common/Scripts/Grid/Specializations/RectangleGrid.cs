using System;
using System.Collections.Generic;
using Common.Grid.Path;
using Common.Grid.Path.Specializations;
using Common.Grid.Physics;
using UnityEngine;
using Common.Grid.Physics.Specializations;

namespace Common.Grid.Specializations
{
    public class RectangleGrid<TContext, TTile> : IGridControl<GridPosition2D, TContext, TTile> where TTile : GridTile<GridPosition2D, TContext, TTile>
    {
        protected readonly List<TTile> m_Tiles = new List<TTile>();

        protected int m_SizeX;
        protected int m_SizeY;
        protected bool m_AllowMoveDiagonally = true;
        protected float m_NonDiagonalHeuristicFactor = 1.0f;
        protected float m_DiagonalHeuristicFactor = (float)Math.Sqrt(2.0);

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

        public RectangleGrid()
        {
            m_TilePhysicalShape = new RectangleGridTilePhysicalShape(m_TileSize);
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
        
        public bool TryGetTile(GridPosition2D i_Position, out TTile o_Tile)
        {
#if !GRID_COORDINATE_SAFETY_DISABLE
            if (i_Position.X >= 0 && i_Position.X < m_SizeX && i_Position.Y >= 0 && i_Position.Y < m_SizeY)
            {
#endif
                int tilesIndex = i_Position.X * m_SizeY + i_Position.Y;
                o_Tile = m_Tiles[tilesIndex];
                return true;
#if !GRID_COORDINATE_SAFETY_DISABLE
            }
            o_Tile = null;
            return false;
#endif
        }


        public void GetConnected(GridPosition2D i_Position, List<TTile> o_ConnectedTiles)
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

        public GridPath<GridPosition2D, TContext, TTile> GetPath(GridPosition2D i_Start, GridPosition2D i_End, TContext i_Context)
        {
            const int pathingDataMargin = 4;
            var pathData = GridPathData2DProvider<TContext, TTile>.GLOBAL.GetGridPathData();

            int minX = Math.Max(i_Start.X - pathingDataMargin, 0);
            int minY = Math.Max(i_Start.Y - pathingDataMargin, 0);
            int maxX = Math.Min(i_End.X - pathingDataMargin, m_SizeX - 1);
            int maxY = Math.Min(i_End.Y - pathingDataMargin, m_SizeY - 1);

            pathData.Set(this, new GridPosition2D(minX, minY), new GridPosition2D(maxX, maxY));
            return new GridPath<GridPosition2D, TContext, TTile>(this, pathData, i_Start, i_End, i_Context);
        }

        public GridPathArea<GridPosition2D, TContext, TTile> GetPathArea(GridPosition2D i_Min, GridPosition2D i_Max, GridPosition2D i_Origin, TContext i_Context)
        {
            var pathData = GridPathData2DProvider<TContext, TTile>.GLOBAL.GetGridPathData();
            pathData.Set(this, i_Min, i_Max);
            return new GridPathArea<GridPosition2D, TContext, TTile>(this, pathData, i_Min, i_Max, i_Origin, i_Context);
        }

        public bool TryGetTilePhysicalData(GridPosition2D i_Position, out GridTilePhysicalData o_Tile)
        {
            Vector3 worldPos = new Vector3();
            worldPos.x = m_GridOrigin.x + i_Position.X * m_TileSize.x;
            worldPos.y = m_GridOrigin.y + i_Position.Y * m_TileSize.y;
            worldPos.z = m_GridOrigin.z;
            o_Tile = new GridTilePhysicalData(worldPos, m_TilePhysicalShape);

#if !GRID_COORDINATE_SAFETY_DISABLE
            return i_Position.X >= 0 && i_Position.X < m_SizeX && i_Position.Y >= 0 && i_Position.Y < m_SizeY;
#else
            return true;
#endif
        }

        public void GetIntersectionsBetween(
            GridPosition2D i_Source, GridPosition2D i_Target,
            List<GridTileRayIntersection<GridPosition2D, TContext, TTile>> o_TilesBetweenPositions,
            Vector3 i_SourceOffset = new Vector3(), Vector3 i_TargetOffset = new Vector3())
        {
            GridTilePhysicalData sourceData;
            if (TryGetTilePhysicalData(i_Source, out sourceData))
            {
                GridTilePhysicalData targetData;
                if (TryGetTilePhysicalData(i_Source, out targetData))
                {
                    var line = i_Target - i_Source;
                    int xDirection = line.X >= 0 ? 1 : -1;
                    int yDirection = line.Y >= 0 ? 1 : -1;

                    Vector3 rayOrigin = sourceData.Position + i_SourceOffset;
                    rayOrigin.y += sourceData.Shape.EdgeFaces[0].VerticalLength * 0.5f;
                    Vector3 rayTarget = targetData.Position + i_TargetOffset;
                    rayTarget.y += targetData.Shape.EdgeFaces[0].VerticalLength * 0.5f;
                    Vector3 rayDirection = rayTarget - rayOrigin;
                    rayDirection.Normalize();

                    int xLast = i_Source.X;
                    int yLast = i_Source.Y;
                    int xNext = i_Source.X + xDirection;
                    int yNext = i_Source.Y + yDirection;
                    int axis = 0;
                    const int AXIS_COUNT = 2;
                    GridTilePhysicalData physicalData;
                    TTile tempTile;
                    TileIntersection intersection;
                    GridPosition2D testLocation = new GridPosition2D(xNext, yLast);
                    bool intersectionFound = false;

                    while (xLast != i_Target.X && yLast != i_Target.Y)
                    {
                        switch (axis)
                        {
                            case 0:
                                testLocation = new GridPosition2D(xNext, yLast);
                                break;
                            case 1:
                                testLocation = new GridPosition2D(xLast, yNext);
                                break;
                            case 2:
                                testLocation = new GridPosition2D(xLast, yLast);
                                break;
                        }
                        if (TryGetTilePhysicalData(testLocation, out physicalData))
                        {
                            physicalData.Intersects(rayOrigin, rayDirection, out intersection);
                            if (intersection.IntersectionType != TileIntersectionType.None)
                            {
                                intersectionFound = true;
                                if (TryGetTile(testLocation, out tempTile))
                                {
                                    o_TilesBetweenPositions.Add(new GridTileRayIntersection<GridPosition2D, TContext, TTile>(tempTile, physicalData, intersection));
                                }
                                switch (axis)
                                {
                                    case 0:
                                        xLast = xNext;
                                        xNext += xDirection;
                                        break;
                                    case 1:
                                        yLast = yNext;
                                        yNext += yDirection;
                                        break;
                                }
                            }
                            else
                            {
                                axis = ++axis % AXIS_COUNT;
                                if (axis == 0 && intersectionFound)
                                {
                                    intersectionFound = false;
                                }
                                else
                                {
                                    Log.ProductionLogError("GetIntersectionsBetween() Abort: Scanning failed to progress. An offset out of tile shape bounds was likely used.");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            Log.ProductionLogError("GetIntersectionsBetween() Abort: Tile physical data for location " + testLocation + " was not found");
                            return;
                        }
                    }
                }
            }
        }
    }
}
