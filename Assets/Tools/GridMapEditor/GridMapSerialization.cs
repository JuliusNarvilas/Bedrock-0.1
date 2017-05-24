using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tools
{
    public class GridMapSerialization
    {



        public void Serialize(GridMapEditorBehaviour i_Editor)
        {
            foreach (Transform child in i_Editor.transform)
            {
                var objectComponent = child.gameObject.GetComponent<GridMapObjectBehaviour>();
                if (objectComponent != null)
                {
                    SerializeGridObject(objectComponent);
                }
            }
        }

        private void SerializeGridObject(GridMapObjectBehaviour i_Child)
        {

        }
    }
}
