using Common.Properties.Numerical;
using Common.Properties.Numerical.Specialisations;
using NUnit.Framework;

namespace Common.Tests.Properties.Numerical
{
    public class NumericalPropertyChangeBundleTests
    {
        private class NumericalPropertyIntTestUpdateCounterModifier :
        NumericalPropertyIntModifierReader, INumericalPropertyModifier<int, int, INumericalPropertyModifierReader<int>>
        {
            public int CallCounter;

            public NumericalPropertyIntTestUpdateCounterModifier() : base(0)
            {
                CallCounter = 0;
            }

            public int GetOrder()
            {
                return 0;
            }

            public INumericalPropertyModifierReader<int> GetReader()
            {
                return this;
            }

            public void Update(ref NumericalPropertyChangeEventStruct<int, int, INumericalPropertyModifierReader<int>> i_EventData)
            {
                ++CallCounter;
            }
        }

        [Test]
        public void NumericalPropertyChangeBundleSingleUpdateTest()
        {
            NumericalPropertyInt<int> numericalProperty = new NumericalPropertyInt<int>(10);
            var counterModifier = new NumericalPropertyIntTestUpdateCounterModifier();
            using (var bundle = numericalProperty.CreateChangeBundle())
            {
                numericalProperty.AddModifier(counterModifier);

                var tempModifier = new NumericalPropertyIntTestUpdateCounterModifier();
                bundle.AddModifier(tempModifier);
                bundle.SetBaseValue(7);

                Assert.That(numericalProperty.Get() == 10, "Numerical property ChangeBundle changes propagated too early.");
            }
            Assert.That(numericalProperty.Get() == 7, "Numerical property ChangeBundle changes propagated incorrectly.");
            Assert.That(counterModifier.CallCounter == 2, "Numerical property ChangeBundle change count incorrect.");
        }
    }
}
