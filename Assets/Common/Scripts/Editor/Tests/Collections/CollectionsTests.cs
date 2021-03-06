﻿using Common.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

            List<float> floatList2 = new List<float>();
            List<double> doubleList2 = new List<double>();
            List<int> intList2 = new List<int>();
            List<long> longList2 = new List<long>();
            List<short> shortList2 = new List<short>();
            List<byte> byteList2 = new List<byte>();
            List<uint> uintList2 = new List<uint>();
            List<ulong> ulongList2 = new List<ulong>();
            List<ushort> ushortList2 = new List<ushort>();
            List<sbyte> sbyteList2 = new List<sbyte>();

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

            floatList2.AddRange(floatList);
            doubleList2.AddRange(doubleList);
            intList2.AddRange(intList);
            longList2.AddRange(longList);
            shortList2.AddRange(shortList);
            byteList2.AddRange(byteList);
            uintList2.AddRange(uintList);
            ulongList2.AddRange(ulongList);
            ushortList2.AddRange(ushortList);
            sbyteList2.AddRange(sbyteList);

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

            floatList2.Sort();
            doubleList2.Sort();
            intList2.Sort();
            longList2.Sort();
            shortList2.Sort();
            byteList2.Sort();
            uintList2.Sort();
            ulongList2.Sort();
            ushortList2.Sort();
            sbyteList2.Sort();

            genericList.InsertionSort(new TestAscendingComparerInt());

            for (int i = 0; i < testSize; ++i)
            {
                Assert.That(floatList[i] == floatList2[i], "Float insertion sort ascending specialization failed.");
                Assert.That(doubleList[i] == doubleList2[i], "Double insertion sort ascending specialization failed.");
                Assert.That(intList[i] == intList2[i], "Int insertion sort ascending specialization failed.");
                Assert.That(longList[i] == longList2[i], "Long insertion sort ascending specialization failed.");
                Assert.That(shortList[i] == shortList2[i], "Short insertion sort ascending specialization failed.");
                Assert.That(byteList[i] == byteList2[i], "Byte insertion sort ascending specialization failed.");
                Assert.That(uintList[i] == uintList2[i], "Uint insertion sort ascending specialization failed.");
                Assert.That(ulongList[i] == ulongList2[i], "Ulong insertion sort ascending specialization failed.");
                Assert.That(ushortList[i] == ushortList2[i], "UShort insertion sort ascending specialization failed.");
                Assert.That(sbyteList[i] == sbyteList2[i], "Sbyte insertion sort ascending specialization failed.");

                Assert.That(genericList[i] == intList[i], "Generic insertion sort ascending failed.");
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
            
            floatList2.Sort((a, b) => -1 * a.CompareTo(b));
            doubleList2.Sort((a, b) => -1 * a.CompareTo(b));
            intList2.Sort((a, b) => -1 * a.CompareTo(b));
            longList2.Sort((a, b) => -1 * a.CompareTo(b));
            shortList2.Sort((a, b) => -1 * a.CompareTo(b));
            byteList2.Sort((a, b) => -1 * a.CompareTo(b));
            uintList2.Sort((a, b) => -1 * a.CompareTo(b));
            ulongList2.Sort((a, b) => -1 * a.CompareTo(b));
            ushortList2.Sort((a, b) => -1 * a.CompareTo(b));
            sbyteList2.Sort((a, b) => -1 * a.CompareTo(b));

            genericList.InsertionSort(new TestDescendingComparerInt());

            for (int i = 1; i < testSize; ++i)
            {
                Assert.That(floatList[i] == floatList2[i], "Float insertion sort descending specialization failed.");
                Assert.That(doubleList[i] == doubleList2[i], "Double insertion sort descending specialization failed.");
                Assert.That(intList[i] == intList2[i], "Int insertion sort descending specialization failed.");
                Assert.That(longList[i] == longList2[i], "Long insertion sort descending specialization failed.");
                Assert.That(shortList[i] == shortList2[i], "Short insertion sort descending specialization failed.");
                Assert.That(byteList[i] == byteList2[i], "Byte insertion sort descending specialization failed.");
                Assert.That(uintList[i] == uintList2[i], "Uint insertion sort descending specialization failed.");
                Assert.That(ulongList[i] == ulongList2[i], "Ulong insertion sort descending specialization failed.");
                Assert.That(ushortList[i] == ushortList2[i], "UShort insertion sort descending specialization failed.");
                Assert.That(sbyteList[i] == sbyteList2[i], "Sbyte insertion sort descending specialization failed.");

                Assert.That(genericList[i] == intList2[i], "Generic insertion sort descending failed.");
            }
        }


        private class Integer : IComparable<Integer>
        {
            public readonly int Value;
            public Integer(int value) { Value = value; }

            public int CompareTo(Integer other)
            {
                return Value.CompareTo(other.Value);
            }
        }

        //[Test]
        public void InsertionSortPerformance()
        {
            const int TEST_SIZE = 1000000;
            List<int> regularIntSortList = new List<int>(TEST_SIZE);
            List<int> insertionIntSortList = new List<int>(TEST_SIZE);
            List<Integer> regularObjSortList = new List<Integer>(TEST_SIZE);
            List<Integer> insertionObjSortList = new List<Integer>(TEST_SIZE);

            int counter = 1;
            for(int i = 0; i < TEST_SIZE; ++i)
            {
                regularIntSortList.Add(counter);
                insertionIntSortList.Add(counter);
                regularObjSortList.Add(new Integer(counter));
                insertionObjSortList.Add(new Integer(counter));
                counter++;
            }
            
            Stopwatch regularIntStopwatch = new Stopwatch();
            Stopwatch insertionIntStopwatch = new Stopwatch();
            Stopwatch regularObjStopwatch = new Stopwatch();
            Stopwatch insertionObjStopwatch = new Stopwatch();

            regularIntStopwatch.Start();
            regularIntSortList.Sort();
            regularIntStopwatch.Stop();

            insertionIntStopwatch.Start();
            insertionIntSortList.InsertionSortAscending();
            insertionIntStopwatch.Stop();

            regularObjStopwatch.Start();
            regularObjSortList.Sort();
            regularObjStopwatch.Stop();

            insertionObjStopwatch.Start();
            insertionObjSortList.InsertionSort();
            insertionObjStopwatch.Stop();

            UnityEngine.Debug.LogFormat("[Sorted Test] Regular int: {0}; Insertion int: {1}; Regular obj: {2}; Insertion obj: {3}",
                regularIntStopwatch.ElapsedMilliseconds,
                insertionIntStopwatch.ElapsedMilliseconds,
                regularObjStopwatch.ElapsedMilliseconds,
                insertionObjStopwatch.ElapsedMilliseconds);

            regularIntSortList[TEST_SIZE - 1] = 0;
            insertionIntSortList[TEST_SIZE - 1] = 0;
            regularObjSortList[TEST_SIZE - 1] = new Integer(0);
            insertionObjSortList[TEST_SIZE - 1] = new Integer(0);

            Stopwatch regularIntStopwatch2 = new Stopwatch();
            Stopwatch insertionIntStopwatch2 = new Stopwatch();
            Stopwatch regularObjStopwatch2 = new Stopwatch();
            Stopwatch insertionObjStopwatch2 = new Stopwatch();

            regularIntStopwatch2.Start();
            regularIntSortList.Sort();
            regularIntStopwatch2.Stop();

            insertionIntStopwatch2.Start();
            insertionIntSortList.InsertionSortAscending();
            insertionIntStopwatch2.Stop();

            regularObjStopwatch2.Start();
            regularObjSortList.Sort();
            regularObjStopwatch2.Stop();

            insertionObjStopwatch2.Start();
            insertionObjSortList.InsertionSort();
            insertionObjStopwatch2.Stop();

            UnityEngine.Debug.LogFormat("[Semi Sorted Test] Regular int: {0}; Insertion int: {1}; Regular obj: {2}; Insertion obj: {3}",
                regularIntStopwatch2.ElapsedMilliseconds,
                insertionIntStopwatch2.ElapsedMilliseconds,
                regularObjStopwatch2.ElapsedMilliseconds,
                insertionObjStopwatch2.ElapsedMilliseconds);
        }
    }
}
