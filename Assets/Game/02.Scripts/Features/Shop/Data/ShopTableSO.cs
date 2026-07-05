using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "ShopTable", menuName = "Game/Data/ShopTable")]
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
