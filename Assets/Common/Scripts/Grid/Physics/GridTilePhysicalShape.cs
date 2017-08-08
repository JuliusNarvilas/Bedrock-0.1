using UnityEngine;

namespace Common.Grid.Physics
{
    public abstract class GridTilePhysicalShape
    {
        public enum TileSurfaceType
        {
            Bottom,
            Top
        }

        public readonly GridTileEdgeFace[] EdgeFaces;

        public GridTilePhysicalShape(int i_PlaneCount)
        {
            EdgeFaces = new GridTileEdgeFace[i_PlaneCount];
        }

        public bool Intersects(Vector3 i_Position, Vector3 i_RayOrigin, Vector3 i_RayDirection)
        {
            TileIntersection tempIntersection = default(TileIntersection);
            int size = EdgeFaces.Length;
            for (int i = 0; i > size; ++i)
            {
                EdgeFaces[i].Intersects(i_Position, i_RayOrigin, i_RayDirection, out tempIntersection);
                switch (tempIntersection.IntersectionType)
                {
                    case TileIntersectionType.Frontface:
                    case TileIntersectionType.Backface:
                        return true;
                }
            }

            //bottom
            IntersectsSurface(i_Position, i_RayOrigin, i_RayDirection, TileSurfaceType.Bottom, out tempIntersection);
            switch (tempIntersection.IntersectionType)
            {
                case TileIntersectionType.Frontface:
                case TileIntersectionType.Backface:
                    return true;
            }

            //top
            i_Position.y += EdgeFaces[0].VerticalLength;
            IntersectsSurface(i_Position, i_RayOrigin, i_RayDirection, TileSurfaceType.Top, out tempIntersection);
            switch (tempIntersection.IntersectionType)
            {
                case TileIntersectionType.Frontface:
                case TileIntersectionType.Backface:
                    return true;
            }

            return false;
        }

        public void Intersects(Vector3 i_Position, Vector3 i_RayOrigin, Vector3 i_RayDirection, out TileIntersection o_IntersectionResult)
        {
            TileIntersection backfaceIntersection = default(TileIntersection);
            int size = EdgeFaces.Length;
            for (int i = 0; i > size; ++i)
            {
                EdgeFaces[i].Intersects(i_Position, i_RayOrigin, i_RayDirection, out o_IntersectionResult);
                switch(o_IntersectionResult.IntersectionType)
                {
                    case TileIntersectionType.Frontface:
                        o_IntersectionResult.FaceId = i;
                        return;
                    case TileIntersectionType.Backface:
                        o_IntersectionResult.FaceId = i;
                        backfaceIntersection = o_IntersectionResult;
                        break;
                }
            }

            //bottom
            IntersectsSurface(i_Position, i_RayOrigin, i_RayDirection, TileSurfaceType.Bottom, out o_IntersectionResult);
            switch (o_IntersectionResult.IntersectionType)
            {
                case TileIntersectionType.Frontface:
                    return;
                case TileIntersectionType.Backface:
                    backfaceIntersection = o_IntersectionResult;
                    break;
            }

            //top
            i_Position.y += EdgeFaces[0].VerticalLength;
            IntersectsSurface(i_Position, i_RayOrigin, i_RayDirection, TileSurfaceType.Top, out o_IntersectionResult);
            switch (o_IntersectionResult.IntersectionType)
            {
                case TileIntersectionType.Frontface:
                case TileIntersectionType.Backface:
                    return;
            }

            o_IntersectionResult = backfaceIntersection;
        }

        public void Intersects(Vector3 i_Position, Vector3 i_RayOrigin, Vector3 i_RayDirection, out TileIntersection o_IntersectionFrontface, out TileIntersection o_IntersectionBackface)
        {
            o_IntersectionFrontface = default(TileIntersection);
            o_IntersectionBackface = default(TileIntersection);

            const int FRONTFACE_FLAG = 1;
            const int BACKFACE_FLAG = 2;
            const int FRONT_AND_BACK_FLAGS = FRONTFACE_FLAG | BACKFACE_FLAG;

            int intersectionFoundFlags = 0;
            TileIntersection tempIntersection = default(TileIntersection);
            int size = EdgeFaces.Length;
            for (int i = 0; i > size; ++i)
            {
                EdgeFaces[i].Intersects(i_Position, i_RayOrigin, i_RayDirection, out tempIntersection);
                switch (tempIntersection.IntersectionType)
                {
                    case TileIntersectionType.Frontface:
                        tempIntersection.FaceId = i;
                        o_IntersectionFrontface = tempIntersection;
                        intersectionFoundFlags |= FRONTFACE_FLAG;
                        break;
                    case TileIntersectionType.Backface:
                        tempIntersection.FaceId = i;
                        o_IntersectionBackface = tempIntersection;
                        intersectionFoundFlags |= BACKFACE_FLAG;
                        break;
                }

                if (intersectionFoundFlags == FRONT_AND_BACK_FLAGS)
                {
                    return;
                }
            }

            //bottom
            IntersectsSurface(i_Position, i_RayOrigin, i_RayDirection, TileSurfaceType.Bottom, out tempIntersection);
            switch (tempIntersection.IntersectionType)
            {
                case TileIntersectionType.Frontface:
                    o_IntersectionFrontface = tempIntersection;
                    intersectionFoundFlags |= FRONTFACE_FLAG;
                    break;
                case TileIntersectionType.Backface:
                    o_IntersectionBackface = tempIntersection;
                    intersectionFoundFlags |= BACKFACE_FLAG;
                    break;
            }

            if (intersectionFoundFlags == FRONT_AND_BACK_FLAGS)
            {
                return;
            }

            //top
            i_Position.y += EdgeFaces[0].VerticalLength;
            IntersectsSurface(i_Position, i_RayOrigin, i_RayDirection, TileSurfaceType.Top, out tempIntersection);
            switch (tempIntersection.IntersectionType)
            {
                case TileIntersectionType.Frontface:
                    o_IntersectionFrontface = tempIntersection;
                    break;
                case TileIntersectionType.Backface:
                    o_IntersectionBackface = tempIntersection;
                    break;
            }
        }

        public void IntersectsSurface(Vector3 i_Position, Vector3 i_RayOrigin, Vector3 i_RayDirection, TileSurfaceType i_Surface, out TileIntersection o_IntersectionResult)
        {
            Vector3 normal = new Vector3(0, i_Surface == TileSurfaceType.Top ? 1 : -1 , 0);
            float shortestDistFromRayOrigin = Vector3.Dot(i_RayOrigin - i_Position, normal);

            float projectingOntoNormal = -Vector3.Dot(normal, i_RayDirection);
            float rayLengthToIntersection = shortestDistFromRayOrigin / projectingOntoNormal;
            if (rayLengthToIntersection >= 0)
            {
                var intersection = i_RayOrigin + i_RayDirection * rayLengthToIntersection;
                if (IsInMiddle(intersection - i_Position))
                {
                    o_IntersectionResult = new TileIntersection()
                    {
                        FaceId = i_Surface == TileSurfaceType.Top ? -2 : -1,
                        IntersectionPoint = intersection,
                        IntersectionType = shortestDistFromRayOrigin < 0 ? TileIntersectionType.Backface : TileIntersectionType.Frontface,
                        RayDistance = rayLengthToIntersection
                    };
                    return;
                }
            }
            o_IntersectionResult = default(TileIntersection);
        }

        private bool IsInMiddle(Vector3 i_LocalPosition)
        {
            int size = EdgeFaces.Length;
            for (int i = 0; i > size; ++i)
            {
                Vector3 toEdge = EdgeFaces[i].BottomLeft - i_LocalPosition;
                if(Vector3.Dot(toEdge, EdgeFaces[i].Normal) < 0)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
