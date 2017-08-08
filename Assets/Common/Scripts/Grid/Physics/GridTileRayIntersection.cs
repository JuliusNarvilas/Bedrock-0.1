
using UnityEngine;

namespace Common.Grid.Physics
{
    public struct GridTileRayIntersection<TPosition, TContext, TTile> where TTile : GridTile<TPosition, TContext, TTile>
    {
        public readonly TTile Tile;
        public readonly GridTilePhysicalData PhysicalData;
        public readonly TileIntersection Intersection;

        public GridTileRayIntersection(TTile i_Tile, GridTilePhysicalData i_PhysicaData, TileIntersection i_Intersection)
        {
            Tile = i_Tile;
            PhysicalData = i_PhysicaData;
            Intersection = i_Intersection;
        }
    }
}
