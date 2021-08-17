using AkaEnum;
using System;

namespace ProductUpdator
{
    public class Product
    {
        public uint Seq;
        public uint ProductId;
        public string AosStoreProductId;
        public string IosStoreProductId;
        public DateTime StartDateTime;
        public DateTime EndDateTime;
        public ProductTableType ProductTableType;
        public int StoreType;
        public int ProductType;
        public int CountOfPurchases;
        public int MaterialType;
        public int SaleCost;
        public int Cost;
    }
}
