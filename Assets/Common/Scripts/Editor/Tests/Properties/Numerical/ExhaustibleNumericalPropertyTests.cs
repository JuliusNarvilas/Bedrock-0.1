using Common.Properties.Numerical;
using Common.Properties.Numerical.Specialisations;
using NUnit.Framework;

namespace Common.Tests.Properties.Numerical
{
    public class ExhaustibleNumericalPropertyTests
    {
        private class NumericalPropertyIntTestEventDataModifier :
        NumericalPropertyIntModifierReader, INumericalPropertyModifier<int, int, INumericalPropertyModifierReader<int>>
        {
            public bool ConditionMet;

            private int m_OldModifier;
            private int m_OldDepletion;

            public NumericalPropertyIntTestEventDataModifier(int i_OldModifier, int i_OldDepletion) : base(0)
            {
                ConditionMet = false;

                m_OldModifier = i_OldModifier;
                m_OldDepletion = i_OldDepletion;
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
                ConditionMet = (i_EventData.OldDepletion == m_OldDepletion) && (i_EventData.OldModifier == m_OldModifier);
            }
        }

        [Test]
        public void ExhaustibleNumericalPropertyUpdateEventData()
        {
            ExhaustibleNumericalPropertyInt<int> numericalProperty = new ExhaustibleNumericalPropertyInt<int>(2);

            var modifier1 = new NumericalPropertyIntModifier<int>(-3, 1);
            var modifier2 = new NumericalPropertyIntModifier<int>(7, 0);
            var modifier3 = new NumericalPropertyIntModifier<int>(7, 3);
            numericalProperty.AddModifier(modifier1);
            numericalProperty.AddModifier(modifier2);
            numericalProperty.AddModifier(modifier3);
            numericalProperty.Deplete(2);
            numericalProperty.Deplete(3);

            var evenetUpdateTest = new NumericalPropertyIntTestEventDataModifier(11, 5);
            numericalProperty.AddModifier(evenetUpdateTest);

            Assert.That(evenetUpdateTest.ConditionMet, "Numerical property change event data incorrect.");
        }
    }
}
