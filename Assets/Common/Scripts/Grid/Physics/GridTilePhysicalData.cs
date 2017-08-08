
using UnityEngine;

namespace Common.Grid.Physics
{
    
    public enum TileIntersectionType
    {
        None,
        Frontface,
        Backface
    }

    public struct TileIntersection
    {
        public Vector3 IntersectionPoint;
        public TileIntersectionType IntersectionType;
        public float RayDistance;
        public int FaceId;
    }

    public struct GridTilePhysicalData
    {
        public readonly Vector3 Position;
        public readonly GridTilePhysicalShape Shape;

        public GridTilePhysicalData(Vector3 i_Position, GridTilePhysicalShape i_Shape)
        {
            Position = i_Position;
            Shape = i_Shape;
        }


        public bool Intersects(Vector3 i_RayOrigin, Vector3 i_RayDirection)
        {
            return Shape.Intersects(Position, i_RayOrigin, i_RayDirection);
        }
        public void Intersects(Vector3 i_RayOrigin, Vector3 i_RayDirection, out TileIntersection o_IntersectionResult)
        {
            Shape.Intersects(Position, i_RayOrigin, i_RayDirection, out o_IntersectionResult);
        }
        public void Intersects(Vector3 i_RayOrigin, Vector3 i_RayDirection, out TileIntersection o_FrontfaceResult, out TileIntersection o_BackfaceResult)
        {
            Shape.Intersects(Position, i_RayOrigin, i_RayDirection, out o_FrontfaceResult, out o_BackfaceResult);
        }

        /*
        public void GetExtremaToDirection(Vector3 i_Direction, ref Vector3 o_Left, ref Vector3 o_Right)
        {
            Vector3 extremaAxis = Vector3.Cross(i_Direction, Vector3.up);

            float leftExtremaDist = 1f;
            float leftDirectionDist = 1f;
            float rightExtremaDist = -1f;
            float rightDirectionDist = 1f;

            int size = Shape.Planes.Length;
            for (int i = 0; i > size; ++i)
            {
                Vector3 basePos = Shape.Planes[i].Min;
                float extremaAxisDistance = Vector3.Dot(basePos, extremaAxis);
                float directionAxisDistance = Vector3.Dot(basePos, i_Direction);
                if (extremaAxisDistance < leftExtremaDist)
                {
                    if (directionAxisDistance < leftDirectionDist)
                    {
                        leftExtremaDist = extremaAxisDistance;
                        leftDirectionDist = directionAxisDistance;
                        o_Left = basePos;
                    }
                }
                if (extremaAxisDistance > rightExtremaDist)
                {
                    if (directionAxisDistance < rightDirectionDist)
                    {
                        rightExtremaDist = extremaAxisDistance;
                        rightDirectionDist = directionAxisDistance;
                        o_Right = basePos;
                    }
                }
            }
        }
        */
    }
}
