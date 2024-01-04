using System;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


namespace Shop
{
    static class ReflectionInfo
    {
        public static void GetMethods<T>(T c)
        {
            Type Type = c.GetType();
            MethodInfo[] info = Type.GetMethods();
            Console.WriteLine($"{new string('-', 20)} Methods in {Type.Name} {new string('-', 20)}");
            foreach (var method in info)
            {
                Console.WriteLine(method.Name);
            }
        }
        public static void SetField<T,R>(T c, string Field, R value)
        {
            Type Type = c.GetType();
            FieldInfo field = Type.GetField(Field, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(c, value);
            Console.WriteLine($"{new string('-', 20)} Set Value in private field: {field.Name} {new string('-', 20)}");
        }
    }

    public class F : IName, IPrice, ISerializable
    {
        string IName.Name => throw new NotImplementedException();

        decimal IPrice.Price => throw new NotImplementedException();

        event EventHandler<PriceEventData> IPrice.update
        {
            add
            {
                throw new NotImplementedException();
            }
            
            remove
            {
                throw new NotImplementedException();
            }
        }

        int IComparable<IName>.CompareTo(IName other)
        {
            throw new NotImplementedException();
        }

        void ISerializable.GetObjectData(BinaryWriter _writer)
        {
            throw new NotImplementedException();
        }

        void ISerializable.SetObjectData(BinaryReader _reader)
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {

        static void Main()
        {
            try
            { 
                Container<Product> container = new Container<Product>(){new VideoEquipment(), new Computer()};
                Container<Product> container_2 = new Container<Product>(){new VideoEquipment("Camera",200m,"Sony",2021,"1920x1080")};
                FileStream stream = File.Create("Serialization.dat");
                using (stream)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, container);
                    formatter.Serialize(stream, container_2);
                }

                using (stream = new FileStream("Serialization.dat", FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    var _container = Activator.CreateInstance(container.GetType(), null);
                    var _container_2 = Activator.CreateInstance(container.GetType(), null);
                    _container = formatter.Deserialize(stream);
                    _container_2 = formatter.Deserialize(stream);
                    Console.WriteLine(_container.ToString());
                    Console.WriteLine("\n\n");
                    Console.WriteLine(_container_2.ToString());
                }

            }
            catch (ProductException ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
                if (ex.Value != null)
                {
                    Console.WriteLine($"Некоректне значення: {ex.Value}");
                }

            }

        }
    }
}

