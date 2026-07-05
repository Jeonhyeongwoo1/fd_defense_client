using System;

namespace Game.Data
{
    public enum ShopProductType
    {
        UnitUnlock
    }

    [Serializable]
    public class ShopData
    {
        public string id;
        public ShopProductType productType;
        public string targetId;
        public int price;
        public int displayOrder;
    }
}
