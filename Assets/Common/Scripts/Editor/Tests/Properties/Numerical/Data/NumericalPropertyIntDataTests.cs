using NUnit.Framework;
using System;
using Common.Properties.Numerical.Data;

namespace Common.Tests.Properties.Numerical.Data
{
    public class NumericalPropertyIntDataTests
    {
        [Test]
        public void NumericalPropertyIntDataConstructors()
        {
            NumericalPropertyIntData emptyPropertyDataValue = new NumericalPropertyIntData();
            int initialEmptyPropertyDataInt = emptyPropertyDataValue.Get();
            Assert.That(initialEmptyPropertyDataInt == 0);

            Random rnd = new Random();
            int randomIntSource = rnd.Next(1, 1000);
            NumericalPropertyIntData propertyDataValue = new NumericalPropertyIntData(randomIntSource);
            int initialPropertyDataValue = propertyDataValue.Get();
            Assert.That(randomIntSource == initialPropertyDataValue);
        }
        [Test]
        public void NumericalPropertyIntDataCreation()
        {
            Random rnd = new Random();
            int randomIntSource = rnd.Next(1, 1000);
            NumericalPropertyIntData propertyDataValue = new NumericalPropertyIntData(randomIntSource);

            INumericalPropertyData<int> createdPropertyDataValue = propertyDataValue.CreateZero();
            Assert.That(!ReferenceEquals(createdPropertyDataValue, propertyDataValue));
            Assert.That(createdPropertyDataValue.Get() == 0);
        }

        [Test]
        public void NumericalPropertyIntDataAdd()
        {
            Random rnd = new Random();
            int randomIntSource1 = rnd.Next(1, 1000);
            int randomIntSource2 = rnd.Next(1, 1000);
            NumericalPropertyIntData propertyDataValue = new NumericalPropertyIntData(randomIntSource1);

            propertyDataValue.Add(randomIntSource2);
            int sum = propertyDataValue.Get();
            Assert.That(sum == (randomIntSource1 + randomIntSource2));
        }

        [Test]
        public void NumericalPropertyIntDataSubstract()
        {
            Random rnd = new Random();
            int randomIntSource1 = rnd.Next(1, 1000);
            int randomIntSource2 = rnd.Next(1, 1000);
            NumericalPropertyIntData propertyDataValue = new NumericalPropertyIntData(randomIntSource1);

            propertyDataValue.Substract(randomIntSource2);
            int substracted = propertyDataValue.Get();
            Assert.That(substracted == (randomIntSource1 - randomIntSource2));
        }

        [Test]
        public void NumericalPropertyIntDataMultiply()
        {
            Random rnd = new Random();
            int randomIntSource1 = rnd.Next(1, 1000);
            int randomIntSource2 = rnd.Next(1, 1000);
            NumericalPropertyIntData propertyDataValue = new NumericalPropertyIntData(randomIntSource1);

            propertyDataValue.Multiply(randomIntSource2);
            int multiplied = propertyDataValue.Get();
            Assert.That(multiplied == (randomIntSource1 * randomIntSource2));
        }

        [Test]
        public void NumericalPropertyIntDataSet()
        {
            Random rnd = new Random();
            int randomIntSource1 = rnd.Next(1, 1000);
            NumericalPropertyIntData propertyDataValue = new NumericalPropertyIntData();

            propertyDataValue.Set(randomIntSource1);
            Assert.That(propertyDataValue.Get() == randomIntSource1);
        }

        [Test]
        public void NumericalPropertyIntDataInverse()
        {
            Random rnd = new Random();
            int randomIntSource1 = rnd.Next(1, 1000);
            NumericalPropertyIntData propertyDataValue = new NumericalPropertyIntData(randomIntSource1);

            int additiveInverse = propertyDataValue.AdditiveInverse();
            Assert.That(additiveInverse == -propertyDataValue.Get());

            Assert.That(propertyDataValue.MultiplicativeInverse() == 0);
        }

        [Test]
        public void NumericalPropertyIntDataToZero()
        {
            Random rnd = new Random();
            int randomIntSource1 = rnd.Next(1, 1000);
            NumericalPropertyIntData propertyDataValue = new NumericalPropertyIntData(randomIntSource1);

            propertyDataValue.ToZero();
            Assert.That(propertyDataValue.Get() == 0);
        }

        [Test]
        public void NumericalPropertyIntDataDivide()
        {
            Random rnd = new Random();
            int randomIntSource1 = rnd.Next(1, 1000);
            int randomIntSource2 = rnd.Next(1, 1000);
            NumericalPropertyIntData propertyDataValue = new NumericalPropertyIntData(randomIntSource1);
            
            propertyDataValue.Divide(randomIntSource2);
            int divided = propertyDataValue.Get();
            Assert.That(divided == (randomIntSource1 / randomIntSource2));
        }
    }
}
