using Common.Grid.Specializations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common.Grid.Editor
{
    public class TestCuboidGrid : CuboidGrid<int, TestGridTile>
    {
        private int m_GenerateX = 0;
        private int m_GenerateY = 0;
        private int m_GenerateZ = 0;

        private GridPosition3D GetNextPosition()
        {
            var result = new GridPosition3D(m_GenerateX, m_GenerateY, m_GenerateZ);
            if (++m_GenerateZ >= m_SizeZ)
            {
                m_GenerateZ = 0;
                if (++m_GenerateY >= m_SizeY)
                {
                    m_GenerateY = 0;
                    ++m_GenerateX;
                }
            }
            return result;
        }

        private string m_GridCostData =
            "1; 1;" +
            "1; 1;" +
            "1; 1;" +

            "2; 1;" +
            "1; 1;" +
            "1; 1;" +
            
            "5; 5;" +
            "5; 1;" +
            "1; 1;" +
            
            "2; 1;" +
            "1; 1;" +
            "1; 1;" +
            
            "1; 1;" +
            "1; 1;" +
            "1; 1";

        public TestCuboidGrid() : base(5, 3, 2, new Vector3(1, 2, 1))
        {
            var costs = m_GridCostData.Split(';');

            for(int i = 0; i < costs.Length; ++i)
            {
                int cost = int.Parse(costs[i]);
                var newTile = new TestGridTile(GetNextPosition(), cost);
                m_Tiles.Add(newTile);
            }
        }
    }
}
