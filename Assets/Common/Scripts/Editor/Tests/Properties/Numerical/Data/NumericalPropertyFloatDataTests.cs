using NUnit.Framework;
using System;
using Common.Properties.Numerical.Data;

namespace Common.Tests.Properties.Numerical.Data
{
    public class NumericalPropertyFloatDataTests
    {
        [Test]
        public void NumericalPropertyFloatDataConstructors()
        {
            NumericalPropertyFloatData emptyPropertyDataValue = new NumericalPropertyFloatData();
            float initialEmptyPropertyDataVal = emptyPropertyDataValue.Get();
            Assert.That(initialEmptyPropertyDataVal == 0);

            Random rnd = new Random();
            float randomNumberSource = rnd.Next(1, 1000);
            NumericalPropertyFloatData propertyDataValue = new NumericalPropertyFloatData(randomNumberSource);
            float initialPropertyDataValue = propertyDataValue.Get();
            Assert.That(randomNumberSource == initialPropertyDataValue);
        }
        [Test]
        public void NumericalPropertyFloatDataCreation()
        {
            Random rnd = new Random();
            float randomNumberSource = rnd.Next(1, 1000);
            NumericalPropertyFloatData propertyDataValue = new NumericalPropertyFloatData(randomNumberSource);

            INumericalPropertyData<float> createdPropertyDataValue = propertyDataValue.CreateZero();
            Assert.That(!ReferenceEquals(createdPropertyDataValue, propertyDataValue));
            Assert.That(createdPropertyDataValue.Get() == 0);
        }

        [Test]
        public void NumericalPropertyFloatDataAdd()
        {
            Random rnd = new Random();
            float randomNumberSource1 = rnd.Next(1, 1000);
            float randomNumberSource2 = rnd.Next(1, 1000);
            NumericalPropertyFloatData propertyDataValue = new NumericalPropertyFloatData(randomNumberSource1);

            propertyDataValue.Add(randomNumberSource2);
            float sum = propertyDataValue.Get();
            Assert.That(sum == (randomNumberSource1 + randomNumberSource2));
        }

        [Test]
        public void NumericalPropertyFloatDataSubstract()
        {
            Random rnd = new Random();
            float randomNumberSource1 = rnd.Next(1, 1000);
            float randomNumberSource2 = rnd.Next(1, 1000);
            NumericalPropertyFloatData propertyDataValue = new NumericalPropertyFloatData(randomNumberSource1);

            propertyDataValue.Substract(randomNumberSource2);
            float substracted = propertyDataValue.Get();
            Assert.That(substracted == (randomNumberSource1 - randomNumberSource2));
        }

        [Test]
        public void NumericalPropertyFloatDataMultiply()
        {
            Random rnd = new Random();
            float randomNumberSource1 = rnd.Next(1, 1000);
            float randomNumberSource2 = rnd.Next(1, 1000);
            NumericalPropertyFloatData propertyDataValue = new NumericalPropertyFloatData(randomNumberSource1);

            propertyDataValue.Multiply(randomNumberSource2);
            float multiplied = propertyDataValue.Get();
            Assert.That(multiplied == (randomNumberSource1 * randomNumberSource2));
        }

        [Test]
        public void NumericalPropertyFloatDataSet()
        {
            Random rnd = new Random();
            float randomNumberSource1 = rnd.Next(1, 1000);
            NumericalPropertyFloatData propertyDataValue = new NumericalPropertyFloatData();

            propertyDataValue.Set(randomNumberSource1);
            Assert.That(propertyDataValue.Get() == randomNumberSource1);
        }

        [Test]
        public void NumericalPropertyFloatDataInverse()
        {
            Random rnd = new Random();
            float randomNumberSource1 = rnd.Next(1, 1000);
            NumericalPropertyFloatData propertyDataValue = new NumericalPropertyFloatData(randomNumberSource1);

            float additiveInverse = propertyDataValue.AdditiveInverse();
            Assert.That(additiveInverse == -propertyDataValue.Get());

            float multiplicativeInverse = 1.0f / propertyDataValue.Get();
            Assert.That(propertyDataValue.MultiplicativeInverse() == multiplicativeInverse);
        }

        [Test]
        public void NumericalPropertyFloatDataToZero()
        {
            Random rnd = new Random();
            float randomNumberSource1 = rnd.Next(1, 1000);
            NumericalPropertyDoubleData propertyDataValue = new NumericalPropertyDoubleData(randomNumberSource1);

            propertyDataValue.ToZero();
            Assert.That(propertyDataValue.Get() == 0);
        }

        [Test]
        public void NumericalPropertyFloatDataDivide()
        {
            Random rnd = new Random();
            float randomNumberSource1 = rnd.Next(1, 1000);
            float randomNumberSource2 = rnd.Next(1, 1000);
            NumericalPropertyFloatData propertyDataValue = new NumericalPropertyFloatData(randomNumberSource1);
            propertyDataValue.Divide(randomNumberSource2);

            float divided = propertyDataValue.Get();
            Assert.That(divided.EqualsF(randomNumberSource1 / randomNumberSource2));
        }

    }
}
