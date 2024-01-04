
using System;
using System.IO;


namespace Shop
{
    public interface IName : IComparable<IName>
    {
        string Name { get; }
    }
    public interface IPrice
    {
        decimal Price { get; }
        event EventHandler<PriceEventData> update;
    }
    public interface ISerializable
    {
        void GetObjectData(BinaryWriter _writer);
        void SetObjectData(BinaryReader _reader);
    }
    public class PriceEventData : EventArgs
    {
        public PriceEventData(decimal price)
        {
            _price = price;
        }
        public decimal _price { get; }
    }
    [Serializable]
    public abstract class Product: IName, IPrice, ISerializable
    {
        private string _name;
        private decimal _price;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        public decimal Price
        {
            get { return _price; }
            set {
                if (value < 0m)
                    throw new ProductException($"Ціна {value} не є дійсною", value);
                update?.Invoke(this, new PriceEventData(value - _price));
                _price = value;
            }
        }

        public event EventHandler<PriceEventData> update;
        public int CompareTo(IName OtherPbject)
        {
            return this.Name.CompareTo(OtherPbject.Name);
        }
        public Product() : this("None", 0)
        {
        }
        public Product(string name) : this(name, 0)
        {
        }

        public Product(string name = "None_Name", decimal price = 0)
        {
            _name = name;
            _price = price;
        }

        

        public override string ToString()
        {
            return $" Name: {Name}\n Price: {Price}$";
        }
        public virtual void GetObjectData(BinaryWriter BinWriter)
        {
            BinWriter.Write(Name);
            BinWriter.Write(Price);
        }
        public virtual void SetObjectData(BinaryReader BinReader)
        {
            Name = BinReader.ReadString();
            Price = BinReader.ReadDecimal();
        }

    }
    [Serializable]
    public class Electronic : Product
    {
        public string Brand { get; set; }
        public int YearOfIssue { get; set; }

        public Electronic() : base()
        {
            Brand = "None_Name_Brand";
            YearOfIssue = 0;
        }
        public Electronic(string name = "None_Name", decimal price = 0, string brand = "None_Name_Brand") : base(name,price)
        {
            Brand = brand;
            YearOfIssue = 0;
        }

        public Electronic(string name = "None_Name ", decimal price = 0, string brand = "None_Name_Brand ", int year = 0) : base(name, price)
        {
            Brand = brand;
            YearOfIssue = year;
        }

        public override string ToString()
        {

            return $"{base.ToString()} \n  Brand: {Brand}\n  Year of issue: {YearOfIssue}";
        }
        public override void GetObjectData(BinaryWriter BinWriter)
        {
            base.GetObjectData(BinWriter);
            BinWriter.Write(Brand);
            BinWriter.Write(YearOfIssue);
        }
        public override void SetObjectData(BinaryReader BinReader)
        {
            base.SetObjectData(BinReader);
            Brand = BinReader.ReadString();
            YearOfIssue = BinReader.ReadInt32();

        }
    }
    [Serializable]
    public class Computer: Electronic
    {
        public string Processor { get; set; }
        public int RAM { get; set; }
        public int ROM { get; set; }

        public Computer() : base()
        {
            Processor = "None_Name_Processor";
            RAM = 0;
            ROM = 0;
        }
        public Computer(string name = "None_Name", decimal price = 0,string brand = "None_Name_Brand", string processor = "None_Name_Processor") : base(name,price,brand)
        {
            Brand = brand;
            YearOfIssue = 0;
            Processor = processor;
        }

        public Computer(string name = "None_Name ", decimal price = 0, string brand = "None_Name_Brand ", string processor = "None_Name_Processor", int year = 0, int ram =0, int rom =0) : base(name,price,brand,year)
        {
            Brand = brand;
            YearOfIssue = year;
            Processor = processor;
            RAM= ram;
            ROM= rom;
        }
        public override string ToString()
        {
            return $"{base.ToString()}\n    Processor: {Processor}\n    RAM: {RAM}\n    ROM: {ROM}\n";
        }
        public override void GetObjectData(BinaryWriter BinWriter)
        {
            base.GetObjectData(BinWriter);
            BinWriter.Write(Processor);
            BinWriter.Write(RAM);
            BinWriter.Write(ROM);
        }
        public override void SetObjectData(BinaryReader BinReader)
        {
            base.SetObjectData(BinReader);
            Processor = BinReader.ReadString();
            RAM = BinReader.ReadInt32();
            ROM = BinReader.ReadInt32();

        }
    }
    [Serializable]
    public class VideoEquipment : Electronic
    {
        public string Resolution { get; set; } = "None_Resolution";
        public int Diagonal { get; set; }

        public VideoEquipment() : base()
        {
            //Resolution = "None_Resolution";
            Diagonal = 0;
        }
        public VideoEquipment(string name = "None_Name", decimal price = 0, 
                              string brand = "None_Name_Brand", string resolution = "None_Resolution") : base(name, price, brand)
        {
            Resolution = resolution;
        }

        public VideoEquipment(string name = "None_Name ", decimal price = 0, string brand = "None_Name_Brand ",
                              int year = 0,string resolution = "None_Resolution", int diagonal=0) : base(name, price, brand, year)
        {
            Resolution = resolution;
            Diagonal= diagonal;
        }
        public override string ToString()
        {
            return $"{base.ToString()}\n    Resolution: {Resolution}\n    Diagonal: {Diagonal}\n";
        }
        public override void GetObjectData(BinaryWriter BinWriter)
        {
            base.GetObjectData(BinWriter);
            BinWriter.Write(Resolution);
            BinWriter.Write(Diagonal);
        }
        public override void SetObjectData(BinaryReader BinReader)
        {
            base.SetObjectData(BinReader);
            Resolution = BinReader.ReadString();
            Diagonal = BinReader.ReadInt32();
        }
    }
    public class ProductException : Exception
    {
        private object _value;
        public object Value { set { _value = value; } get { return _value; } }
        public ProductException(string msg, object value) : base(msg)
        {
            Value = value;
        }
        public ProductException(string msg) : base(msg) { }
    }
    
}
