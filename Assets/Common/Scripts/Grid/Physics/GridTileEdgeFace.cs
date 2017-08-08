
using UnityEngine;

namespace Common.Grid.Physics
{
    public struct GridTileEdgeFace
    {

        private static readonly Vector3 s_faceVerticalDirection = new Vector3(0, 1, 0);

        public readonly Vector3 BottomLeft;
        public readonly Vector3 Normal;
        public readonly Vector3 HorisontalDirection;
        public readonly float VerticalLength;
        public readonly float HorisontalLength;

        public GridTileEdgeFace(Vector3 i_BottomLeft, Vector3 i_TopRight)
        {
            BottomLeft = i_BottomLeft;

            Vector3 across = i_TopRight - i_BottomLeft;
            VerticalLength = across.y;
            Vector3 horizontalVec = new Vector3(across.x, 0, across.z);
            HorisontalLength = horizontalVec.magnitude;
            HorisontalDirection = horizontalVec / HorisontalLength;

            Normal = Vector3.Cross(s_faceVerticalDirection, HorisontalDirection).normalized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i_Position">The tested face placement</param>
        /// <param name="i_RayOrigin"></param>
        /// <param name="i_RayDirection"></param>
        /// <param name="o_Intersection"></param>
        /// <returns></returns>
        public void Intersects(Vector3 i_Position, Vector3 i_RayOrigin, Vector3 i_RayDirection, out TileIntersection o_Result)
        {
            Vector3 worldBottomLeft = i_Position + BottomLeft;
            float shortestDistFromRayOrigin = Vector3.Dot(i_RayOrigin - worldBottomLeft, Normal);

            float projectingOntoNormal = -Vector3.Dot(Normal, i_RayDirection);
            float rayLengthToIntersection = shortestDistFromRayOrigin / projectingOntoNormal;
            var intersection = i_RayOrigin + i_RayDirection * rayLengthToIntersection;
            o_Result = new TileIntersection()
            {
                IntersectionType = TileIntersectionType.None,
                RayDistance = rayLengthToIntersection,
                IntersectionPoint = intersection
            };
            if(rayLengthToIntersection < 0)
            {
                return;
            }

            Vector3 faceEdgeToIntersection = (intersection - worldBottomLeft);
            float horizontalDistance = Vector3.Dot(faceEdgeToIntersection, HorisontalDirection);
            if (horizontalDistance <= HorisontalLength && horizontalDistance >= 0.0f)
            {
                float verticalDistance = Vector3.Dot(faceEdgeToIntersection, s_faceVerticalDirection);

                if (verticalDistance <= VerticalLength && verticalDistance >= 0.0f)
                {
                    o_Result.IntersectionType = (shortestDistFromRayOrigin <= 0) ? TileIntersectionType.Backface : TileIntersectionType.Frontface;
                }
            }
        }
    }
}
