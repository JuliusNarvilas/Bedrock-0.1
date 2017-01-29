using Common.Properties.Numerical;
using Common.Properties.Numerical.Data;
using Common.Properties.Numerical.Specialisations;
using NUnit.Framework;
using System;

namespace Common.Tests.Properties.Numerical
{
    public class NumericalPropertyTests
    {
        private class NumericalPropertyIntTestNestedUpdateModifier :
        NumericalPropertyIntModifierReader, INumericalPropertyModifier<int, int, INumericalPropertyModifierReader<int>>
        {
            public bool ConditionMet;

            public NumericalPropertyIntTestNestedUpdateModifier() : base(0)
            {
                ConditionMet = false;
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
                if ((i_EventData.EChangeTypeMask & ENumericalPropertyChangeType.NestedUpdate) != 0)
                {
                    if ((i_EventData.EChangeTypeMask & ENumericalPropertyChangeType.ForceUpdate) != 0)
                    {
                        if ((i_EventData.EChangeTypeMask & ENumericalPropertyChangeType.ModifierAdd) == 0)
                        {
                            ConditionMet = true;
                        }
                    }
                }
                else if ((i_EventData.EChangeTypeMask & ENumericalPropertyChangeType.ModifierAdd) != 0)
                {
                    ConditionMet = false;
                    i_EventData.NumericalProperty.Update();
                }
            }
        }


        [Test]
        public void NumericalPropertyConstructors()
        {
            Random rnd = new Random();
            int randomNumber = rnd.Next(1, 10000);
            NumericalPropertyInt<int> numericalProperty = new NumericalPropertyInt<int>(randomNumber);
            NumericalPropertyInt<int> emptyNumericalProperty = new NumericalPropertyInt<int>();

            Assert.That(numericalProperty.Get() == randomNumber, "Initialisation value not set.");
            Assert.That(emptyNumericalProperty.Get() == 0, "Default value not 0.");
        }

        [Test]
        public void NumericalPropertyCreation()
        {
            NumericalPropertyInt<int> numericalProperty = new NumericalPropertyInt<int>();
            INumericalPropertyData<int> newDataZero = numericalProperty.CreateZeroData();

            Assert.That(newDataZero.Get() == 0, "Default value not 0.");
            Assert.That(!ReferenceEquals(newDataZero, numericalProperty), "Not a new instance.");
        }


        [Test]
        public void NumericalPropertyGet()
        {
            NumericalPropertyInt<int> numericalProperty = new NumericalPropertyInt<int>(-10);
            var modifier = new NumericalPropertyIntModifier<int>(5, 0);
            numericalProperty.AddModifier(modifier);

            Assert.That(numericalProperty.Get() == -5, "Numerical property Get is incorrect.");
            Assert.That(numericalProperty.GetPositive() == 0, "Numerical property GetPositive is incorrect.");
            Assert.That(numericalProperty.GetBaseValue() == -10, "Numerical property GetBaseValue is incorrect.");
        }

        [Test]
        public void NumericalPropertySet()
        {
            NumericalPropertyInt<int> numericalProperty = new NumericalPropertyInt<int>(-10);
            var modifier = new NumericalPropertyIntModifier<int>(-3, 0);
            numericalProperty.AddModifier(modifier);
            numericalProperty.SetBaseValue(5);

            Assert.That(numericalProperty.Get() == 2, "Numerical property Get does not recognise SetBase.");
            Assert.That(numericalProperty.GetPositive() == 2, "Numerical property GetPositive does not recognise SetBase.");
            Assert.That(numericalProperty.GetBaseValue() == 5, "Numerical property GetBaseValue does not recognise SetBase.");
        }
        
        [Test]
        public void NumericalPropertyGetModifier()
        {
            NumericalPropertyInt<int> numericalProperty = new NumericalPropertyInt<int>(-10);
            var modifier1 = new NumericalPropertyIntModifier<int>(-3, 1);
            var modifier2 = new NumericalPropertyIntModifier<int>(7, 0);
            numericalProperty.AddModifier(modifier1);
            numericalProperty.AddModifier(modifier2);
            INumericalPropertyModifierReader<int> returnModifier = numericalProperty.GetModifier(1);

            Assert.That(numericalProperty.GetFinalModifier() == 4, "Numerical property GetFinalModifier incorrect.");
            Assert.That(numericalProperty.GetModifierCount() == 2, "Numerical property GetModifierCount incorrect.");
            Assert.That(ReferenceEquals(returnModifier, modifier1), "Numerical property GetModifier returns unexpected reference.");
        }
        
        [Test]
        public void NumericalPropertyRemoveModifier()
        {
            NumericalPropertyInt<int> numericalProperty = new NumericalPropertyInt<int>(-10);
            var modifier1 = new NumericalPropertyIntModifier<int>(-3, 1);
            var modifier2 = new NumericalPropertyIntModifier<int>(7, 0);
            var modifier3 = new NumericalPropertyIntModifier<int>(7, 3);
            numericalProperty.AddModifier(modifier1);
            numericalProperty.AddModifier(modifier2);
            numericalProperty.AddModifier(modifier3);
            numericalProperty.RemoveModifier(modifier1);
            INumericalPropertyModifierReader<int> returnModifier = numericalProperty.GetModifier(0);

            Assert.That(numericalProperty.Get() == 4, "Numerical property Get incorrect.");
            Assert.That(numericalProperty.GetFinalModifier() == 14, "Numerical property GetFinalModifier incorrect.");
            Assert.That(numericalProperty.GetModifierCount() == 2, "Numerical property GetModifierCount incorrect.");
            Assert.That(ReferenceEquals(returnModifier, modifier2), "Numerical property GetModifier returns unexpected reference.");
        }


        [Test]
        public void NumericalPropertyUpdate()
        {
            NumericalPropertyInt<int> numericalProperty = new NumericalPropertyInt<int>();
            var evenetUpdateTest = new NumericalPropertyIntTestNestedUpdateModifier();
            numericalProperty.AddModifier(evenetUpdateTest);
            Assert.That(evenetUpdateTest.ConditionMet, "Numerical property change event nested update incorrect.");
        }
    }
}
