using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Common.IO
{
    public static class SerializationSurrogates
    {
        private interface ICustomSerializationSurrogate : ISerializationSurrogate
        {
            Type Type { get; }
        }

        private sealed class Vector3SerializationSurrogate : ICustomSerializationSurrogate
        {
            public Type Type
            {
                get { return typeof(Vector3); }
            }

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Vector3 v3 = (Vector3)obj;
                info.AddValue("x", v3.x);
                info.AddValue("y", v3.y);
                info.AddValue("z", v3.z);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Vector3 v3 = (Vector3)obj;
                v3.x = (float)info.GetValue("x", typeof(float));
                v3.y = (float)info.GetValue("y", typeof(float));
                v3.z = (float)info.GetValue("z", typeof(float));
                obj = v3;
                return obj;
            }
        }

        private sealed class Vector2SerializationSurrogate : ICustomSerializationSurrogate
        {
            public Type Type
            {
                get { return typeof(Vector2); }
            }

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Vector2 v3 = (Vector2)obj;
                info.AddValue("x", v3.x);
                info.AddValue("y", v3.y);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Vector2 v3 = (Vector2)obj;
                v3.x = (float)info.GetValue("x", typeof(float));
                v3.y = (float)info.GetValue("y", typeof(float));
                obj = v3;
                return obj;
            }
        }

        private sealed class QuaternionSerializationSurrogate : ICustomSerializationSurrogate
        {
            public Type Type
            {
                get { return typeof(Quaternion); }
            }

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Quaternion val = (Quaternion)obj;
                info.AddValue("x", val.x);
                info.AddValue("y", val.y);
                info.AddValue("z", val.z);
                info.AddValue("w", val.w);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Quaternion val = (Quaternion)obj;
                val.x = (float)info.GetValue("x", typeof(float));
                val.y = (float)info.GetValue("y", typeof(float));
                val.z = (float)info.GetValue("z", typeof(float));
                val.w = (float)info.GetValue("w", typeof(float));
                obj = val;
                return obj;
            }
        }

        private sealed class ColorSerializationSurrogate : ICustomSerializationSurrogate
        {
            public Type Type
            {
                get { return typeof(Color); }
            }

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Color val = (Color)obj;
                info.AddValue("a", val.a);
                info.AddValue("b", val.b);
                info.AddValue("g", val.g);
                info.AddValue("r", val.r);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Color val = (Color)obj;
                val.a = (float)info.GetValue("a", typeof(float));
                val.b = (float)info.GetValue("b", typeof(float));
                val.g = (float)info.GetValue("g", typeof(float));
                val.r = (float)info.GetValue("r", typeof(float));
                obj = val;
                return obj;
            }
        }


        private static void AddSurrogate(ICustomSerializationSurrogate i_Surrogate)
        {
            s_Surrogates.AddSurrogate(i_Surrogate.Type, new StreamingContext(StreamingContextStates.All), i_Surrogate);
        }


        private static SurrogateSelector s_Surrogates = null;


        public static SurrogateSelector GetSurrogateSelector()
        {
            if(s_Surrogates != null)
            {
                return s_Surrogates;
            }

            s_Surrogates = new SurrogateSelector();
            AddSurrogate(new Vector3SerializationSurrogate());
            AddSurrogate(new Vector2SerializationSurrogate());
            AddSurrogate(new QuaternionSerializationSurrogate());
            AddSurrogate(new ColorSerializationSurrogate());

            return s_Surrogates;
        }

    }
}
