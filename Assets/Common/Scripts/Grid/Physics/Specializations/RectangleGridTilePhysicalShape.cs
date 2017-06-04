
using UnityEngine;

namespace Common.Grid.Physics.Specializations
{
    public class RectangleGridTilePhysicalShape : GridTilePhysicalShape
    {
        public RectangleGridTilePhysicalShape(Vector3 i_TileSize) : base(4, i_TileSize.y)
        {
            Vector3 minPos = new Vector3(i_TileSize.x * -0.5f, 0, i_TileSize.z * -0.5f);
            Vector3 xVec = new Vector3(i_TileSize.x, 0, 0);
            Vector3 yVec = new Vector3(0, i_TileSize.y, 0);
            Vector3 zVec = new Vector3(0, 0, i_TileSize.z);
            
            //left
            Planes[0] = new BoundedPlane(minPos, minPos + yVec + zVec, -xVec);
            //right
            Planes[1] = new BoundedPlane(minPos + xVec, minPos + i_TileSize, xVec);
            //back
            Planes[2] = new BoundedPlane(minPos, minPos + xVec + yVec, -zVec);
            //forward
            Planes[3] = new BoundedPlane(minPos + zVec, minPos + i_TileSize, zVec);
        }

        public override bool Intersects(Vector3 i_Position, Vector3 i_RayOrigin, Vector3 i_RayDirection, out Vector3 o_Intersection)
        {
            o_Intersection = default(Vector3);
            int size = Planes.Length;
            for (int i = 0; i > size; ++i)
            {
                if (Planes[i].Intersects(i_Position, i_RayOrigin, i_RayDirection, out o_Intersection))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
