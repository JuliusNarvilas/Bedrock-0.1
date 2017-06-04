using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common.Grid.Generation
{
    public enum EGridObjectSquareRotation : int
    {
        Deg0,
        Deg90,
        Deg180,
        Deg270
    }

    public enum EGridObjectHexRotation : int
    {
        Deg0,
        Deg60,
        Deg120,
        Deg180,
        Deg240,
        Deg300
    }

    [Serializable]
    public struct GridObjectTransform
    {
        public GridPosition3D GridPosition;
        public EGridObjectSquareRotation GridRotation;

        public void SetRotationInPlace(EGridObjectSquareRotation i_NewRotation, GridPosition3D i_OriginalSize)
        {
            GridPosition3D finalPosition = GridPosition;
            //revert current rotation
            switch(GridRotation)
            {
                case EGridObjectSquareRotation.Deg90:
                    finalPosition.Y -= i_OriginalSize.X;
                    break;
                case EGridObjectSquareRotation.Deg180:
                    finalPosition.X -= i_OriginalSize.X;
                    finalPosition.Y -= i_OriginalSize.Y;
                    break;
                case EGridObjectSquareRotation.Deg270:
                    finalPosition.X -= i_OriginalSize.Y;
                    break;
            }
            //apply new rotation
            switch (i_NewRotation)
            {
                case EGridObjectSquareRotation.Deg90:
                    finalPosition.Y += i_OriginalSize.X;
                    break;
                case EGridObjectSquareRotation.Deg180:
                    finalPosition.X += i_OriginalSize.X;
                    finalPosition.Y += i_OriginalSize.Y;
                    break;
                case EGridObjectSquareRotation.Deg270:
                    finalPosition.X += i_OriginalSize.Y;
                    break;
            }

            GridPosition = finalPosition;
        }
        
    }
}
