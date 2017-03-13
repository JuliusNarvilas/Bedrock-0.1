using UnityEngine;
using System;
using System.Reflection;

namespace Common
{
    public class EnumFlagAttribute : PropertyAttribute
    {
        public delegate int ConversionDelegate(int value, bool export);
        public string EnumName;
        public ConversionDelegate Converter;

        public EnumFlagAttribute()
        {
            EnumName = null;
            Converter = DefaultConverter;
        }

        public EnumFlagAttribute(string name)
        {
            EnumName = name;
            Converter = DefaultConverter;
        }

        public EnumFlagAttribute(string name, Type converterType, string converterName)
        {
            EnumName = name;
            Converter = (ConversionDelegate)Delegate.CreateDelegate(converterType, converterType.GetMethod(converterName));
        }

        public EnumFlagAttribute(Type converterType, string converterName)
        {
            EnumName = null;
            var function = converterType.GetMethod(converterName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            Converter = (ConversionDelegate)Delegate.CreateDelegate(typeof(ConversionDelegate), function);
        }


        private int DefaultConverter(int value, bool export)
        {
            return value;
        }
    }
}
