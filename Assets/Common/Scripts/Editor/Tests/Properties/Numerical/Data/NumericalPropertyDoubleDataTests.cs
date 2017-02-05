using NUnit.Framework;
using System;
using Common.Properties.Numerical.Data;

namespace Common.Tests.Properties.Numerical.Data
{
    class NumericalPropertyDoubleDataTests
    {
        [Test]
        public void NumericalPropertyDoubleDataConstructors()
        {
            NumericalPropertyDoubleData emptyPropertyDataValue = new NumericalPropertyDoubleData();
            double initialEmptyPropertyDataVal = emptyPropertyDataValue.Get();
            Assert.That(initialEmptyPropertyDataVal == 0);

            Random rnd = new Random();
            double randomNumberSource = rnd.Next(1, 1000);
            NumericalPropertyDoubleData propertyDataValue = new NumericalPropertyDoubleData(randomNumberSource);
            double initialPropertyDataValue = propertyDataValue.Get();
            Assert.That(randomNumberSource == initialPropertyDataValue);
        }

        [Test]
        public void NumericalPropertyDoubleDataCreation()
        {
            Random rnd = new Random();
            double randomNumberSource = rnd.Next(1, 1000);
            NumericalPropertyDoubleData propertyDataValue = new NumericalPropertyDoubleData(randomNumberSource);

            INumericalPropertyData<double> createdPropertyDataValue = propertyDataValue.CreateZero();
            Assert.That(!ReferenceEquals(createdPropertyDataValue, propertyDataValue));
            Assert.That(createdPropertyDataValue.Get() == 0);
        }

        [Test]
        public void NumericalPropertyDoubleDataAdd()
        {
            Random rnd = new Random();
            double randomNumberSource1 = rnd.Next(1, 1000);
            double randomNumberSource2 = rnd.Next(1, 1000);
            NumericalPropertyDoubleData propertyDataValue = new NumericalPropertyDoubleData(randomNumberSource1);

            propertyDataValue.Add(randomNumberSource2);
            double sum = propertyDataValue.Get();
            Assert.That(sum == (randomNumberSource1 + randomNumberSource2));
        }

        [Test]
        public void NumericalPropertyDoubleDataSubstract()
        {
            Random rnd = new Random();
            double randomNumberSource1 = rnd.Next(1, 1000);
            double randomNumberSource2 = rnd.Next(1, 1000);
            NumericalPropertyDoubleData propertyDataValue = new NumericalPropertyDoubleData(randomNumberSource1);

            propertyDataValue.Substract(randomNumberSource2);
            double substracted = propertyDataValue.Get();
            Assert.That(substracted == (randomNumberSource1 - randomNumberSource2));
        }

        [Test]
        public void NumericalPropertyDoubleDataSet()
        {
            Random rnd = new Random();
            double randomNumberSource1 = rnd.Next(1, 1000);
            double randomNumberSource2 = rnd.Next(1, 1000);
            NumericalPropertyDoubleData propertyDataValue = new NumericalPropertyDoubleData(randomNumberSource1);
            propertyDataValue.Set(randomNumberSource2);
            Assert.That(propertyDataValue.Get() == randomNumberSource2);
        }

        [Test]
        public void NumericalPropertyDoubleDataToZero()
        {
            Random rnd = new Random();
            double randomNumberSource1 = rnd.Next(1, 1000);
            NumericalPropertyDoubleData propertyDataValue = new NumericalPropertyDoubleData(randomNumberSource1);

            propertyDataValue.ToZero();
            Assert.That(propertyDataValue.Get() == 0);
        }
    }
}
