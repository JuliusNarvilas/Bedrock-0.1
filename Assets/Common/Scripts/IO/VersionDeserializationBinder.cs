using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Common.IO
{
    /// <summary>
    /// === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
    /// Do not change this 
    /// </summary>
    /// <seealso cref="System.Runtime.Serialization.SerializationBinder" />
    public sealed class VersionDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
            {
                Type typeToDeserialize = null;

                assemblyName = Assembly.GetExecutingAssembly().FullName;

                // The following line of code returns the type. 
                typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));

                return typeToDeserialize;
            }

            return null;
        }

        public static readonly VersionDeserializationBinder Instance = new VersionDeserializationBinder();
    }
}
