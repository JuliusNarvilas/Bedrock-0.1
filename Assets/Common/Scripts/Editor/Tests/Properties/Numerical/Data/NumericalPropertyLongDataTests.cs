using NUnit.Framework;
using System;
using System.Diagnostics;
using Common.Properties.Numerical.Data;

namespace Common.Tests.Properties.Numerical.Data
{
    class NumericalPropertyLongDataTests
    {

        [Test]
        public void NumericalPropertyLongDataConstructors()
        {
            NumericalPropertyLongData emptyPropertyDataValue = new NumericalPropertyLongData();
            long initialEmptyPropertyDataVal = emptyPropertyDataValue.Get();
            Assert.That(initialEmptyPropertyDataVal == 0);

            Random rnd = new Random();
            long randomNumberSource = rnd.Next(1, 1000);
            NumericalPropertyLongData propertyDataValue = new NumericalPropertyLongData(randomNumberSource);
            long initialPropertyDataValue = propertyDataValue.Get();
            Assert.That(randomNumberSource == initialPropertyDataValue);
        }

        [Test]
        public void NumericalPropertyLongDataCreation()
        {
            NumericalPropertyLongData propertyDataValue = new NumericalPropertyLongData();

            INumericalPropertyData<long> createdPropertyDataValue = propertyDataValue.CreateZero();
            Assert.That(!ReferenceEquals(createdPropertyDataValue, propertyDataValue));
            Assert.That(createdPropertyDataValue.Get() == 0);
        }

        [Test]
        public void NumericalPropertyLongDataAdd()
        {
            Random rnd = new Random();
            long randomNumberSource1 = rnd.Next(1, 1000);
            long randomNumberSource2 = rnd.Next(1, 1000);

            NumericalPropertyLongData propertyDataValue = new NumericalPropertyLongData(randomNumberSource1);
            propertyDataValue.Add(randomNumberSource2);
            long sum = propertyDataValue.Get();
            Assert.That(sum == (randomNumberSource1 + randomNumberSource2));
        }

        [Test]
        public void NumericalPropertyLongDataSubstract()
        {
            Random rnd = new Random();
            long randomNumberSource1 = rnd.Next(1, 1000);
            long randomNumberSource2 = rnd.Next(1, 1000);

            NumericalPropertyLongData propertyDataValue = new NumericalPropertyLongData(randomNumberSource1);
            propertyDataValue.Substract(randomNumberSource2);
            long substracted = propertyDataValue.Get();
            Assert.That(substracted == (randomNumberSource1 - randomNumberSource2));
        }
        
        [Test]
        public void NumericalPropertyLongDataMultiply()
        {
            Random rnd = new Random();
            long randomNumberSource1 = rnd.Next(1, 1000);
            long randomNumberSource2 = rnd.Next(1, 1000);

            NumericalPropertyLongData propertyDataValue = new NumericalPropertyLongData(randomNumberSource1);
            propertyDataValue.Multiply(randomNumberSource2);
            long multiplied = propertyDataValue.Get();
            Assert.That(multiplied == (randomNumberSource1 * randomNumberSource2));
        }

        [Test]
        public void NumericalPropertyLongDataSet()
        {
            Random rnd = new Random();
            long randomNumberSource1 = rnd.Next(1, 1000);

            NumericalPropertyLongData propertyDataValue = new NumericalPropertyLongData(0);
            propertyDataValue.Set(randomNumberSource1);
            Assert.That(propertyDataValue.Get() == randomNumberSource1);
        }

        [Test]
        public void NumericalPropertyLongDataInverse()
        {
            Random rnd = new Random();
            long randomNumberSource1 = rnd.Next(1, 1000);

            NumericalPropertyLongData propertyDataValue = new NumericalPropertyLongData(randomNumberSource1);
            long additiveInverse = propertyDataValue.AdditiveInverse();
            Assert.That(additiveInverse == -propertyDataValue.Get());

            Assert.That(propertyDataValue.MultiplicativeInverse() == 0);
        }

        [Test]
        public void NumericalPropertyLongDataDivide()
        {
            Random rnd = new Random();
            long randomNumberSource1 = rnd.Next(1, 1000);
            long randomNumberSource2 = rnd.Next(1, 1000);
            NumericalPropertyLongData propertyDataValue = new NumericalPropertyLongData(randomNumberSource1);

            propertyDataValue.Divide(randomNumberSource2);
            long divided = propertyDataValue.Get();
            long manuallyDivided = (randomNumberSource1 / randomNumberSource2);
            Assert.That(divided == manuallyDivided);
        }

        [Test]
        public void NumericalPropertyLongDataToZero()
        {
            Random rnd = new Random();
            long randomNumberSource1 = rnd.Next(1, 1000);
            NumericalPropertyLongData propertyDataValue = new NumericalPropertyLongData(randomNumberSource1);

            propertyDataValue.ToZero();
            Assert.That(propertyDataValue.Get() == 0);
        }
    }
}
