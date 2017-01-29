using NUnit.Framework;
using System;

namespace Common.Tests
{
    public class MathHelperTests
    {
        [Test]
        public void FloatEquals()
        {
            Random rnd = new Random();
            float randomNumberSource1 = rnd.Next(1, 1000);

            float limit = MathHelper.DEFAULT_FLOAT_EQUALITY_ERROR_MARGIN;
            bool limitMinEquals = MathHelper.EqualsWithMargin(randomNumberSource1, randomNumberSource1 - limit);
            bool limitMaxEquals = MathHelper.EqualsWithMargin(randomNumberSource1, randomNumberSource1 + limit);

            bool overLimitMin = MathHelper.EqualsWithMargin(randomNumberSource1, randomNumberSource1 - limit - limit);
            bool overLimitMax = MathHelper.EqualsWithMargin(randomNumberSource1, randomNumberSource1 + limit + limit);

            Assert.That(limitMinEquals && limitMaxEquals && !overLimitMin && !overLimitMax);
        }
    }
}