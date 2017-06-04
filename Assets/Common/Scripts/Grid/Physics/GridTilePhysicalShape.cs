using UnityEngine;

namespace Common.Grid.Physics
{
    public abstract class GridTilePhysicalShape
    {
        public readonly BoundedPlane[] Planes;
        public readonly float Height;

        public GridTilePhysicalShape(int i_PlaneCount, float i_Height)
        {
            Planes = new BoundedPlane[i_PlaneCount];
            Height = i_Height;
        }

        public abstract bool Intersects(Vector3 i_Position, Vector3 i_RayOrigin, Vector3 i_RayDirection, out Vector3 o_Intersection);
    }
}
