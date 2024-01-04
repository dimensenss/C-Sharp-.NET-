using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Shop
{
    [Serializable]
    public class Container<T> : IEnumerable<T>, ISerializable where T : IName, IPrice, ISerializable
    {
        public void GetObjectData(BinaryWriter BinWriter)
        {
            BinWriter.Write(ArrayItems.Length);
            foreach(T item in ArrayItems)
            {
                BinWriter.Write(item.GetType().Assembly.ToString());
                BinWriter.Write(item.GetType().FullName);
                item.GetObjectData(BinWriter);
            }
        }
        public void SetObjectData(BinaryReader BinReader)
        {
            int amount = BinReader.ReadInt32();
            for(int i = 0; i < amount;i++)
            {
                string AssemblyName = BinReader.ReadString();
                string FullName = BinReader.ReadString();
                Type newT = Assembly.Load(AssemblyName).GetType(FullName);
                var item = (T)Activator.CreateInstance(newT, null);
                item.SetObjectData(BinReader);
                Add(item);
            }
            //while (BinReader.BaseStream.Position < BinReader.BaseStream.Length)
            //{
            //    string AssemblyName = BinReader.ReadString();
            //    string FullName = BinReader.ReadString();
            //    Type newT = Assembly.Load(AssemblyName).GetType(FullName);
            //    var item = (T)Activator.CreateInstance(newT, null);
            //    item.SetObjectData(BinReader);
            //    AddToContainer(item);
            //}
        }

        private int count = 0;
        private T[] ArrayItems;
        public int GetCount { get { return count; } }

        private decimal totalPrice;
        public void PriceUpdate(object s, PriceEventData p)
        {
            totalPrice += p._price;
        }

        public decimal TotalPrice { get { return totalPrice; } }
        public Container()
        {
            ArrayItems = new T[0];
        }
        public void Add(T item)
        {
            item.update += PriceUpdate;
            T[] NewArrayItems = new T[ArrayItems.Length + 1];
            for (int i = 0; i < ArrayItems.Length; i++)
            {
                NewArrayItems[i] = ArrayItems[i];
            }
            NewArrayItems[ArrayItems.Length] = item;
            ArrayItems = NewArrayItems;
            count = ArrayItems.Length;

            totalPrice += item.Price;
        }

        public void RemoveFromContainer(int index)
        {
            if (index < 0 || index >= count)
            {
               
              throw new Exception($"Видалення за індексом. Iндекс виходить за межi массиву");
            }
            T[] NewArrayItems = new T[count - 1];
            decimal NewTotalPrice = 0;
            for (int i = 0, j = 0; i < count; i++)
            {
                if (i != index)
                {
                    
                    NewTotalPrice += ArrayItems[i].Price;
                    NewArrayItems[j++] = ArrayItems[i];
                }
                else
                {
                    ArrayItems[i].update -= PriceUpdate;
                }
            }
            totalPrice = NewTotalPrice;
            ArrayItems = NewArrayItems;
            count = ArrayItems.Length;
        }
        public void SortByPrice()
        {
            for (int i = 0; i < count - 1; i++)
            {
                for (int j = 0; j < count - i - 1; j++)
                {
                    if (ArrayItems[j].Price > ArrayItems[j + 1].Price)
                    {
                        T temp = ArrayItems[j];
                        ArrayItems[j] = ArrayItems[j + 1];
                        ArrayItems[j + 1] = temp;
                    }
                }
            }
        }
        public override string ToString()
        {
            string s = "";
            foreach (T item in ArrayItems)
                s += item.ToString() + "\n";
            return s;
        }

        public T this[int index]
        {
            get
            {
                if (index <= 0 || index > ArrayItems.Length)
                {
                   
                    throw new Exception($"Iндексатор по порядковому номеру. Iндекс виходить за межi массиву");
                }
                else
                    return ArrayItems[--index];
            }

        }
        public T this[string name]
        {
            get
            {
                foreach (T Item in ArrayItems)
                {
                    if (Item.Name == name)
                        return Item;
                }
                
                throw new Exception($"Iндексатор на назвi. Назва товару не знайдена");
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in ArrayItems)
            {
                yield return item;
            }
        }
        public IEnumerable<T> SubstrIterator(string substr)
        {
            foreach (T i in this)
            {
                if (i.Name.Contains(substr))
                    yield return i;
            }
        }
        public void SaveToFile()
        {
            //string fileName = $"{GetType().Name}.txt";
            string fileName = $"{nameof(Container<T>)}.txt";
            try
            {
                using (StreamWriter outfile = new StreamWriter(fileName))
                {
                    foreach (T item in ArrayItems)
                    {
                        outfile.WriteLine(item.ToString());
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
        public delegate int CompDeleg<T>(T a, T b) where T : IName;
        public void Sort(CompDeleg<T> comp = null)
        {
            for (int i = 0; i < count - 1; i++)
            {
                for (int j = 0; j < count - i - 1; j++)
                {
                    int swap = 0;
                    if (comp != null)
                        swap = comp(ArrayItems[j], ArrayItems[j + 1]);

                    if (swap > 0)
                    {
                        T temp = ArrayItems[j];
                        ArrayItems[j] = ArrayItems[j + 1];
                        ArrayItems[j + 1] = temp;
                    }
                }
            }
        }
        public delegate bool FindDeleg<T>(T a) where T : IName;
        public IEnumerable<T> FindAll(FindDeleg<T> find)
        {
            foreach (T el in ArrayItems)
            {
                if (find(el))
                    yield return el;
            }
        }

        
        //public IName GetDearestItem
        //{
        //    get { 
        //        var DearestItem = (from item in ArrayItems where(item as Product).Price == 
        //                          (from item_1 in ArrayItems select item_1).Max((a) => ((a as Product).Price)) select item).First();
        //        return DearestItem;
        //    }
        //}
        //public IName GetCheapestItem
        //{
        //    get { 
        //        var CheapestItem = (from item in ArrayItems where (item as Product).Price ==
        //                           (from item_1 in ArrayItems select item_1).Min((a) => ((item as Product).Price)) select item).First();
        //        return CheapestItem;
        //    }
        //}
        //public IEnumerable GetAveragePriceFromCategory
        //{
        //    get
        //    {
        //        var query = from item in ArrayItems group item by item.GetType() into types 
        //                    select new { key = types.Key, avg = types.Average((a) => (a as Product).Price) };
        //        foreach (var a in query)
        //        {
        //            yield return a.key;
        //            yield return a.avg;
        //        }
        //    }
        //}
    }
}
