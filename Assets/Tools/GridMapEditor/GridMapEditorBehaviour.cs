using Common.Grid;
using Game.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{

    public class GridMapEditorBehaviour : MonoBehaviour
    {
        public static readonly Color NORMAL_TILE_COLOR = new Color(0f, 0f, 1f, 0.5f);
        public static readonly Color EMPTY_TILE_COLOR = new Color(0.7f, 0.7f, 1f, 0.5f);
        public static readonly Color SELECTED_TILE_COLOR = new Color(1f, 1f, 0f, 0.5f);
        public static readonly Color SELECTED_EMPTY_TILE_COLOR = new Color(1f, 1f, 0.7f, 0.5f);
        public GridPosition3D Size;
        public Vector3 TileSize;

        private Vector3 m_LastTileSize;

        public void DrawGridMapObject(GridMapObjectBehaviour i_Obj)
        {
            int instanceId = i_Obj.GetInstanceID();
            GridPosition3D origin = i_Obj.GetFinalPosition();
            
            int tileCount = i_Obj.Tiles.Count;
            for(int i = 0; i < tileCount; ++i)
            {
                var tile = i_Obj.Tiles[i];
                bool selected = (GridMapObjectBehaviour.ActiveGridObject == instanceId) && (GridMapObjectBehaviour.ActiveGridTileIndex == i);
                
                DrawTileDisplay(origin + tile.Position, (int)tile.TileSettings | (int)tile.TileBlockerSettings, selected);
            }

            if (GridMapObjectBehaviour.ActiveGridObject == instanceId)
                Gizmos.color = Color.yellow;
            else
                Gizmos.color = NORMAL_TILE_COLOR;

            DrawObjectAreaDisplay(origin, i_Obj.Size);
        }

        private void DrawTileDisplay(GridPosition3D i_FinalPosition, int i_Settings, bool i_Selected)
        {
            Vector3 finalGlobalPosition = transform.position;
            finalGlobalPosition.x += i_FinalPosition.X * TileSize.x;
            finalGlobalPosition.y += i_FinalPosition.Z * TileSize.y;
            finalGlobalPosition.z += i_FinalPosition.Y * TileSize.z;
            
            Vector4 heights = GetHeights((GridTileBlockerFlags)i_Settings) * TileSize.y;

            Vector3 fullBlockerSize = new Vector3(0.1f, TileSize.y, TileSize.z * 0.9f);
            Vector3 rotatedFullBlockerSize = Quaternion.Euler(0, 90, 0) * fullBlockerSize;
            
            Vector3 finalBasePos = finalGlobalPosition;
            finalBasePos.x += 0.5f * TileSize.x;
            finalBasePos.z += 0.5f * TileSize.z;
            if((i_Settings & (int)GridTileBlockerFlags.BottomBlocker) == (int)GridTileBlockerFlags.BottomBlocker)
            {
                if (i_Selected)
                    Gizmos.color = SELECTED_TILE_COLOR;
                else
                    Gizmos.color = NORMAL_TILE_COLOR;

                Gizmos.DrawCube(finalBasePos, new Vector3(0.9f * TileSize.x, 0.1f * TileSize.y, 0.9f * TileSize.z));
            }
            else
            {
                if (i_Selected)
                    Gizmos.color = SELECTED_EMPTY_TILE_COLOR;
                else
                    Gizmos.color = EMPTY_TILE_COLOR;

                Gizmos.DrawCube(finalBasePos, new Vector3(0.9f * TileSize.x, 0f, 0.9f * TileSize.z));
            }


            if (i_Selected)
                Gizmos.color = SELECTED_TILE_COLOR;
            else
                Gizmos.color = NORMAL_TILE_COLOR;

            if (heights.x > 0.001f)
            {
                Vector3 size = fullBlockerSize;
                size.y = heights.x;
                Vector3 blockerPos = finalGlobalPosition + (size * 0.5f);
                Gizmos.DrawCube(blockerPos, size);
            }
            if (heights.y > 0.001f)
            {
                Vector3 size = rotatedFullBlockerSize;
                size.y = heights.y;
                Vector3 blockerPos = finalGlobalPosition + (size * 0.5f);
                blockerPos.z += TileSize.z;
                Gizmos.DrawCube(blockerPos, size);
            }
            if (heights.z > 0.001f)
            {
                Vector3 size = fullBlockerSize;
                size.y = heights.z;
                Vector3 blockerPos = finalGlobalPosition + (size * 0.5f);
                blockerPos.x += TileSize.x - fullBlockerSize.x;
                Gizmos.DrawCube(blockerPos, size);
            }
            if (heights.w > 0.001f)
            {
                Vector3 size = rotatedFullBlockerSize;
                size.y = heights.w;
                Vector3 blockerPos = finalGlobalPosition + (size * 0.5f);
                Gizmos.DrawCube(blockerPos, size);
            }
        }

        private static Vector4 GetHeights(GridTileBlockerFlags blockerFlags)
        {
            Vector4 heights;
            heights.x = GetBlockerDirectionHeight(blockerFlags, GridTileBlockerLocations.LeftBlocker);
            heights.y = GetBlockerDirectionHeight(blockerFlags, GridTileBlockerLocations.ForwardBlocker);
            heights.z = GetBlockerDirectionHeight(blockerFlags, GridTileBlockerLocations.RightBlocker);
            heights.w = GetBlockerDirectionHeight(blockerFlags, GridTileBlockerLocations.BackwardBlocker);
            
            return heights;
        }

        private static float GetBlockerDirectionHeight(GridTileBlockerFlags blockerFlags, GridTileBlockerLocations location)
        {
            int stride = (int)GridTileBlockerHeights.BitStride * (int)location;
            if ((((int)GridTileBlockerHeights.FullySetStride << stride) & (int)blockerFlags) != 0)
            {
                if ((((int)GridTileBlockerHeights.ExtraLargeBlocker << stride) & (int)blockerFlags) == ((int)GridTileBlockerHeights.ExtraLargeBlocker << stride))
                    return 1.0f;
                else if ((((int)GridTileBlockerHeights.LargeBlocker << stride) & (int)blockerFlags) == ((int)GridTileBlockerHeights.LargeBlocker << stride))
                    return 0.75f;
                else if ((((int)GridTileBlockerHeights.MediumBlocker << stride) & (int)blockerFlags) == ((int)GridTileBlockerHeights.MediumBlocker << stride))
                    return 0.5f;
                else if ((((int)GridTileBlockerHeights.SmallBlocker << stride) & (int)blockerFlags) == ((int)GridTileBlockerHeights.SmallBlocker << stride))
                    return 0.25f;
            }
            return 0.0f;
        }

        private void DrawObjectAreaDisplay(GridPosition3D i_FinalPosition, GridPosition3D i_Size)
        {
            Vector3 finalGlobalPosition = transform.position;
            finalGlobalPosition.x += (i_FinalPosition.X) * TileSize.x;
            finalGlobalPosition.y += (i_FinalPosition.Z) * TileSize.y;
            finalGlobalPosition.z += (i_FinalPosition.Y) * TileSize.z;

            Vector3 sizeVec = new Vector3(i_Size.X * TileSize.x, i_Size.Z * TileSize.y, i_Size.Y * TileSize.z);
            finalGlobalPosition += sizeVec * 0.5f;
            
            Gizmos.DrawWireCube(finalGlobalPosition, sizeVec);
        }
        
        public void OnValidate()
        {
            if (m_LastTileSize != TileSize)
            {
                m_LastTileSize = TileSize;
                var gridMapObjects = GetComponentsInChildren<GridMapObjectBehaviour>();

                foreach (var gridObject in gridMapObjects)
                {
                    gridObject.SnapToGrid(false);
                }
            }
        }
    }
}