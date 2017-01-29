using Common.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Common.Tests.Collections
{
    public class CollectionsTests
    {
        private class TestAscendingComparerInt : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x.CompareTo(y);
            }
        }
        private class TestDescendingComparerInt : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x.CompareTo(y) * -1;
            }
        }

        [Test]
        public void InsertionSort()
        {
            List<int> temp = new List<int>();

            Random rnd = new Random();
            int testSize = 1000;
            for (int i = 0; i < testSize; ++i)
            {
                temp.Add(rnd.Next(0, 128));
            }

            List<float> floatList = new List<float>();
            List<double> doubleList = new List<double>();
            List<int> intList = new List<int>();
            List<long> longList = new List<long>();
            List<short> shortList = new List<short>();
            List<byte> byteList = new List<byte>();
            List<uint> uintList = new List<uint>();
            List<ulong> ulongList = new List<ulong>();
            List<ushort> ushortList = new List<ushort>();
            List<sbyte> sbyteList = new List<sbyte>();

            List<int> genericList = new List<int>();

            foreach (int value in temp)
            {
                floatList.Add(value);
                doubleList.Add(value);
                intList.Add(value);
                longList.Add(value);
                shortList.Add((short)value);
                byteList.Add((byte)value);
                uintList.Add((uint)value);
                ulongList.Add((ulong)value);
                ushortList.Add((ushort)value);
                sbyteList.Add((sbyte)value);

                genericList.Add(value);
            }

            floatList.InsertionSortAscending();
            doubleList.InsertionSortAscending();
            intList.InsertionSortAscending();
            longList.InsertionSortAscending();
            shortList.InsertionSortAscending();
            byteList.InsertionSortAscending();
            uintList.InsertionSortAscending();
            ulongList.InsertionSortAscending();
            ushortList.InsertionSortAscending();
            sbyteList.InsertionSortAscending();

            genericList.InsertionSort(new TestAscendingComparerInt());

            for (int i = 1; i < testSize; ++i)
            {
                Assert.That(floatList[i - 1] <= floatList[i], "Float insertion sort ascending specialisation failed.");
                Assert.That(doubleList[i - 1] <= doubleList[i], "Double insertion sort ascending specialisation failed.");
                Assert.That(intList[i - 1] <= intList[i], "Int insertion sort ascending specialisation failed.");
                Assert.That(longList[i - 1] <= longList[i], "Long insertion sort ascending specialisation failed.");
                Assert.That(shortList[i - 1] <= shortList[i], "Short insertion sort ascending specialisation failed.");
                Assert.That(byteList[i - 1] <= byteList[i], "Byte insertion sort ascending specialisation failed.");
                Assert.That(uintList[i - 1] <= uintList[i], "Uint insertion sort ascending specialisation failed.");
                Assert.That(ulongList[i - 1] <= ulongList[i], "Ulong insertion sort ascending specialisation failed.");
                Assert.That(ushortList[i - 1] <= ushortList[i], "UShort insertion sort ascending specialisation failed.");
                Assert.That(sbyteList[i - 1] <= sbyteList[i], "Sbyte insertion sort ascending specialisation failed.");

                Assert.That(genericList[i - 1] <= genericList[i], "Generic insertion sort ascending failed.");
            }

            floatList.Clear();
            doubleList.Clear();
            intList.Clear();
            longList.Clear();
            shortList.Clear();
            byteList.Clear();
            uintList.Clear();
            ulongList.Clear();
            ushortList.Clear();
            sbyteList.Clear();

            genericList.Clear();

            foreach (int value in temp)
            {
                floatList.Add(value);
                doubleList.Add(value);
                intList.Add(value);
                longList.Add(value);
                shortList.Add((short)value);
                byteList.Add((byte)value);
                uintList.Add((uint)value);
                ulongList.Add((ulong)value);
                ushortList.Add((ushort)value);
                sbyteList.Add((sbyte)value);

                genericList.Add(value);
            }

            floatList.InsertionSortDescending();
            doubleList.InsertionSortDescending();
            intList.InsertionSortDescending();
            longList.InsertionSortDescending();
            shortList.InsertionSortDescending();
            byteList.InsertionSortDescending();
            uintList.InsertionSortDescending();
            ulongList.InsertionSortDescending();
            ushortList.InsertionSortDescending();
            sbyteList.InsertionSortDescending();

            genericList.InsertionSort(new TestDescendingComparerInt());

            for (int i = 1; i < testSize; ++i)
            {
                Assert.That(floatList[i - 1] >= floatList[i], "Float insertion sort descending specialisation failed.");
                Assert.That(doubleList[i - 1] >= doubleList[i], "Double insertion sort descending specialisation failed.");
                Assert.That(intList[i - 1] >= intList[i], "Int insertion sort descending specialisation failed.");
                Assert.That(longList[i - 1] >= longList[i], "Long insertion sort descending specialisation failed.");
                Assert.That(shortList[i - 1] >= shortList[i], "Short insertion sort descending specialisation failed.");
                Assert.That(byteList[i - 1] >= byteList[i], "Byte insertion sort descending specialisation failed.");
                Assert.That(uintList[i - 1] >= uintList[i], "Uint insertion sort descending specialisation failed.");
                Assert.That(ulongList[i - 1] >= ulongList[i], "Ulong insertion sort descending specialisation failed.");
                Assert.That(ushortList[i - 1] >= ushortList[i], "UShort insertion sort descending specialisation failed.");
                Assert.That(sbyteList[i - 1] >= sbyteList[i], "Sbyte insertion sort descending specialisation failed.");

                Assert.That(genericList[i - 1] >= genericList[i], "Generic insertion sort descending failed.");
            }
        }
    }
}
