using System;
using System.IO;

namespace Product
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
}
