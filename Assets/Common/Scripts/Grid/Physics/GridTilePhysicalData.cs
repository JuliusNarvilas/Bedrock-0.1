
using UnityEngine;

namespace Common.Grid.Physics
{
    public struct GridTilePhysicalData
    {
        public readonly Vector3 Position;
        public readonly GridTilePhysicalShape Shape;

        public GridTilePhysicalData(Vector3 i_Position, GridTilePhysicalShape i_Shape)
        {
            Position = i_Position;
            Shape = i_Shape;
        }

        public bool Intersects(Vector3 i_RayOrigin, Vector3 i_RayDirection, out Vector3 o_Point)
        {
            return Shape.Intersects(Position, i_RayOrigin, i_RayDirection, out o_Point);
        }
    }
}
