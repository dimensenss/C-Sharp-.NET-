using System;
using System.IO;
using System.Reflection;
using Product;
namespace Product
{
    public class Serialization
    {
        public static void Write<T>(T item, string file) where T : ISerializable
        {
            using (FileStream stream = new FileStream(file, FileMode.OpenOrCreate))
            {
                BinaryWriter BinWriter = new BinaryWriter(stream);
                BinWriter.Write(item.GetType().Assembly.ToString());
                BinWriter.Write(item.GetType().FullName);
                item.GetObjectData(BinWriter);
            }
        }
        public static void Read<T>(ref T item, string file) where T : ISerializable
        {
            using (FileStream stream = new FileStream(file, FileMode.Open))
            {
                BinaryReader BinReader = new BinaryReader(stream);
                string AssemblyName = BinReader.ReadString();
                string FullName = BinReader.ReadString();
                Type newT = Assembly.Load(AssemblyName).GetType(FullName);
                item = (T)Activator.CreateInstance(newT, null);
                item.SetObjectData(BinReader);
            }
        }
    }
}
