using System.Collections.Generic;
using Game.Core;
using Game.Data;

namespace Game.Service
{
    public class ShopService
    {
        private readonly ShopTableSO _shopTable;
        private readonly GoldService _goldService;
        private readonly UserDataService _userDataService;
        private readonly EventBus _eventBus;

        public ShopService(ShopTableSO shopTable, GoldService goldService, UserDataService userDataService, EventBus eventBus)
        {
            _shopTable = shopTable;
            _goldService = goldService;
            _userDataService = userDataService;
            _eventBus = eventBus;
        }

        public IReadOnlyList<ShopData> GetProducts()
        {
            return _shopTable.GetSortedProducts();
        }

        public bool IsPurchased(ShopData product)
        {
            if (product.productType == ShopProductType.UnitUnlock)
            {
                return _userDataService.IsUnitOwned(product.targetId);
            }
            return false;
        }

        public bool TryPurchase(string productId)
        {
            var product = _shopTable.GetById(productId);

            if (product == null)
            {
                GameLogger.LogError($"[ShopService] Product not found: {productId}");
                return false;
            }

            if (IsPurchased(product))
            {
                GameLogger.LogWarning($"[ShopService] Product already purchased: {productId}");
                return false;
            }

            if (!_goldService.TrySpend(product.price))
            {
                return false;
            }

            if (product.productType == ShopProductType.UnitUnlock)
            {
                _userDataService.AddOwnedUnit(product.targetId);
                _userDataService.AddPurchase(productId);
                _userDataService.Save();

                _eventBus.Publish(new UnitUnlockedEvent { UnitId = product.targetId });

                GameLogger.Log($"[ShopService] Unit unlocked: {product.targetId}");
            }

            return true;
        }
    }
}
