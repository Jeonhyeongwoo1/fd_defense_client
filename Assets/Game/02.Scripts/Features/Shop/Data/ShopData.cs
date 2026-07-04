using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    [CreateAssetMenu(fileName = "ShopTable", menuName = "Game/ShopTable")]
    public class ShopTableSO : ScriptableObject
    {
        public List<ShopData> ShopDataList = new();

        public ShopData GetById(string id)
        {
            foreach (var data in ShopDataList)
            {
                if (data.id == id)
                {
                    return data;
                }
            }
            return null;
        }

        public IReadOnlyList<ShopData> GetSortedProducts()
        {
            return ShopDataList.OrderBy(d => d.displayOrder).ToList();
        }
    }
}
