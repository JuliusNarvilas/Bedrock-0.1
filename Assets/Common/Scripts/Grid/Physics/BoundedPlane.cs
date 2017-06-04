
using UnityEngine;

namespace Common.Grid.Physics
{
    public struct BoundedPlane
    {
        public readonly Vector3 Min;
        public readonly Vector3 Max;
        public readonly Vector3 Normal;

        public BoundedPlane(Vector3 i_Min, Vector3 i_Max, Vector3 i_Normal)
        {
            Min = i_Min;
            Max = i_Max;
            Normal = i_Normal;
        }

        public bool Intersects(Vector3 i_Position, Vector3 i_RayOrigin, Vector3 i_RayDirection, out Vector3 o_Intersection)
        {
            Vector3 worldMin = i_Position + Min;
            float shortestDistFromRayOrigin = Vector3.Dot(i_RayOrigin - worldMin, Normal);
            if(shortestDistFromRayOrigin <= 0)
            {
                o_Intersection = default(Vector3);
                return false;
            }
            Vector3 worldMax = i_Position + Max;

            float projectingOntoNormal = -Vector3.Dot(Normal, i_RayDirection);
            float rayLengthToIntersection = shortestDistFromRayOrigin / projectingOntoNormal;
            o_Intersection = i_RayOrigin + i_RayDirection * rayLengthToIntersection;
            
            return o_Intersection.x >= worldMin.x && o_Intersection.y >= worldMin.y && o_Intersection.z >= worldMin.z &&
                o_Intersection.x < worldMax.x && o_Intersection.y < worldMax.y && o_Intersection.z < worldMax.z;
        }
    }
}
