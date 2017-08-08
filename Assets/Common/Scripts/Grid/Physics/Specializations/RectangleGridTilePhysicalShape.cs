
using UnityEngine;

namespace Common.Grid.Physics.Specializations
{
    public class RectangleGridTilePhysicalShape : GridTilePhysicalShape
    {
        public RectangleGridTilePhysicalShape(Vector3 i_TileSize) : base(4)
        {
            Vector3 minPos = new Vector3(i_TileSize.x * -0.5f, 0, i_TileSize.z * -0.5f);
            Vector3 xVec = new Vector3(i_TileSize.x, 0, 0);
            Vector3 yVec = new Vector3(0, i_TileSize.y, 0);
            Vector3 zVec = new Vector3(0, 0, i_TileSize.z);
            
            //left
            EdgeFaces[0] = new GridTileEdgeFace(minPos, minPos + yVec + zVec);
            //right
            EdgeFaces[1] = new GridTileEdgeFace(minPos + xVec, minPos + i_TileSize);
            //back
            EdgeFaces[2] = new GridTileEdgeFace(minPos, minPos + xVec + yVec);
            //forward
            EdgeFaces[3] = new GridTileEdgeFace(minPos + zVec, minPos + i_TileSize);
        }
    }
}
