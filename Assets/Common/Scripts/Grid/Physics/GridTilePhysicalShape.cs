using UnityEngine;

namespace Common.Grid.Physics
{
    public abstract class GridTilePhysicalShape
    {
        public BoundedPlane[] Planes;
        
        public abstract bool Intersects(Vector3 i_Position, Vector3 i_RayOrigin, Vector3 i_RayDirection, out Vector3 o_Intersection);
    }
}
