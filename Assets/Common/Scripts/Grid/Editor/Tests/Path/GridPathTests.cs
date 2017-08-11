using Common.Grid.Specializations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common.Grid.Editor.Path
{
    public class GridPathTests
    {
        TestCuboidGrid m_Grid = new TestCuboidGrid();

        public GridPathTests()
        {

        }

        [Test]
        public void Test()
        {
            var path = m_Grid.GetPath(new GridPosition3D(0,0,0), new GridPosition3D(4, 0, 0), 0);

            Debug.Log("Cost: " + path.Tiles.Last().PathCost);

            foreach(var tileElement in path.Tiles)
            {
                Debug.Log(tileElement.Tile.Position.ToString());
            }
        }
    }
}
