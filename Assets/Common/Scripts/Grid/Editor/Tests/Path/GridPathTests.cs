using Common.Grid.Path;
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
        public void PathFinding()
        {
            var path = m_Grid.GetPath(new GridPosition3D(0, 0, 0), new GridPosition3D(4, 0, 0), 0);

            Assert.That(path.Tiles.Last().PathCost == 8, "Cheapest path not found.");
            Assert.That(path.Tiles.Count == 9, "Shortest path not found.");
            /*
            foreach(var tileElement in path.Tiles)
            {
                Debug.Log(tileElement.Tile.Position.ToString());
            }
            */
            var pathTiles = path.Tiles;
            foreach (var tile in pathTiles)
            {
                Assert.That(tile.Tile != null, "Path data not available.");
            }
            path.Dispose();
            foreach (var tile in pathTiles)
            {
                Assert.That(tile.Tile == null, "Path data not cleaned.");
            }

        }
        [Test]
        public void PathArea()
        {
            var areaMin = new GridPosition3D(0, 0, 0);
            var areaMax = new GridPosition3D(4, 2, 1);
            var areaOrigin = new GridPosition3D(1, 1, 1);
            var pathArea = m_Grid.GetPathArea(areaMin, areaMax, areaOrigin, 0);
            Assert.That(pathArea.Min.Equals(areaMin), "Path area min wrong.");
            Assert.That(pathArea.Max.Equals(areaMax), "Path area max wrong.");
            Assert.That(pathArea.Origin.Equals(areaOrigin), "Path area origin wrong.");


            GridPathElement<GridPosition3D, int, TestGridTile> pathElement;
            GridPosition3D inspectedPosition = new GridPosition3D(0, 0, 0);
            var getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success , "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 3, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(0, 0, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 2, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(0, 1, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 2, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(0, 1, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 1, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(0, 2, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 3, "Incorrect path cost for {0}.", inspectedPosition);
            
            inspectedPosition = new GridPosition3D(0, 2, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 2, "Incorrect path cost for {0}.", inspectedPosition);




            inspectedPosition = new GridPosition3D(1, 0, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 3, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(1, 0, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 1, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(1, 1, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 1, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(1, 1, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 0, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(1, 2, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 2, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(1, 2, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 1, "Incorrect path cost for {0}.", inspectedPosition);



            inspectedPosition = new GridPosition3D(2, 0, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 8, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(2, 0, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 6, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(2, 1, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 6, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(2, 1, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 1, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(2, 2, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 3, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(2, 2, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 2, "Incorrect path cost for {0}.", inspectedPosition);




            inspectedPosition = new GridPosition3D(3, 0, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 5, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(3, 0, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 3, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(3, 1, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 3, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(3, 1, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 2, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(3, 2, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 4, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(3, 2, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 3, "Incorrect path cost for {0}.", inspectedPosition);




            inspectedPosition = new GridPosition3D(4, 0, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 5, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(4, 0, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 4, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(4, 1, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 4, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(4, 1, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 3, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(4, 2, 0);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 5, "Incorrect path cost for {0}.", inspectedPosition);

            inspectedPosition = new GridPosition3D(4, 2, 1);
            getResult = pathArea.GridPathData.TryGetElement(inspectedPosition, out pathElement);
            Assert.That(getResult == EGridPathDataResponse.Success, "Position not found: {0}.", inspectedPosition);
            Assert.That(pathElement.PathCost == 4, "Incorrect path cost for {0}.", inspectedPosition);
        }
    }
}
