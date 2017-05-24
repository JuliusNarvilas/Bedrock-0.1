using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Common.IO
{
    public static class Serialization
    {

        public static BinaryFormatter CreateBinaryFormatter()
        {
            var bf = new BinaryFormatter()
            {
                Binder = VersionDeserializationBinder.Instance,
                SurrogateSelector = SerializationSurrogates.GetSurrogateSelector()
            };
            return bf;
        }



        public static void ToBinary(Stream o_Stream, object i_Obj)
        {
            var bf = CreateBinaryFormatter();
            bf.Serialize(o_Stream, i_Obj);
        }
        public static byte[] ToBinary(object i_Obj)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                ToBinary(memStream, i_Obj);
                return memStream.ToArray();
            }
        }
        public static string ToBinaryString(object i_Obj)
        {
            return Convert.ToBase64String(ToBinary(i_Obj));
        }



        public static T FromBinary<T>(Stream i_Stream)
        {
            var bf = CreateBinaryFormatter();
            return (T)bf.Deserialize(i_Stream);
        }
        public static T FromBinary<T>(byte[] bytes)
        {
            using (MemoryStream memStream = new MemoryStream(bytes))
            {
                return FromBinary<T>(memStream);
            }
        }
        public static T FromBinaryString<T>(string base64Str)
        {
            return FromBinary<T>(Convert.FromBase64String(base64Str));
        }




        public static void ToJson(Stream outStream, object obj)
        {
            using (StreamWriter writer = new StreamWriter(outStream))
            {
                string jsonStr = ToJson(obj);
                writer.Write(jsonStr);
                writer.Flush();
            }
        }
        public static string ToJson(object obj)
        {
            return JsonUtility.ToJson(obj);
        }


        public static T FromJson<T>(Stream inStream)
        {
            using (StreamReader reader = new StreamReader(inStream))
            {
                return FromJson<T>(reader.ReadToEnd());
            }
        }
        public static T FromJson<T>(string jsonStr)
        {
            return JsonUtility.FromJson<T>(jsonStr);
        }
    }
}
