using System;
using Game.Core;
using Game.Data;
using Game.Service;
using Game.View;
using UniRx;
using VContainer.Unity;

namespace Game.Presenter
{
    public class ShopPanelPresenter : IStartable, IDisposable
    {
        private readonly UI_ShopPanelView _view;
        private readonly ShopService _shopService;
        private readonly GoldService _goldService;
        private readonly UnitTableSO _unitTable;
        private readonly CompositeDisposable _disposables = new();

        public ShopPanelPresenter(
            UI_ShopPanelView view,
            ShopService shopService,
            GoldService goldService,
            UnitTableSO unitTable)
        {
            _view = view;
            _shopService = shopService;
            _goldService = goldService;
            _unitTable = unitTable;
        }

        public void Start()
        {
            var products = _shopService.GetProducts();
            var shopItems = _view.ShopItems;

            if (shopItems.Length < products.Count)
            {
                GameLogger.LogWarning($"[ShopPanelPresenter] ShopItem count ({shopItems.Length}) < product count ({products.Count})");
            }

            for (var i = 0; i < shopItems.Length && i < products.Count; i++)
            {
                var product = products[i];
                var unitData = _unitTable.GetById(product.targetId);

                if (unitData == null)
                {
                    GameLogger.LogError($"[ShopPanelPresenter] UnitData not found for product: {product.id} (targetId: {product.targetId})");
                    continue;
                }

                var isOwned = _shopService.IsPurchased(product);
                shopItems[i].Bind(unitData.iconSprite, unitData.unitName, product.price, isOwned);

                var item = shopItems[i];
                var productId = product.id;
                item.BuyButton.onClick.AsObservable()
                    .Subscribe(_ => OnBuyClicked(productId))
                    .AddTo(_disposables);
            }

            _goldService.Gold.Subscribe(_ => RefreshButtons()).AddTo(_disposables);

            _view.CloseButton.onClick.AsObservable()
                .Subscribe(_ => OnCloseButtonClicked())
                .AddTo(_disposables);
        }

        public void Refresh()
        {
            var products = _shopService.GetProducts();
            var shopItems = _view.ShopItems;

            for (var i = 0; i < shopItems.Length && i < products.Count; i++)
            {
                var product = products[i];
                var unitData = _unitTable.GetById(product.targetId);

                if (unitData == null)
                {
                    continue;
                }

                var isOwned = _shopService.IsPurchased(product);
                shopItems[i].Bind(unitData.iconSprite, unitData.unitName, product.price, isOwned);
            }

            RefreshButtons();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnBuyClicked(string productId)
        {
            var success = _shopService.TryPurchase(productId);

            if (success)
            {
                Refresh();
            }
            else
            {
                GameLogger.Log("[ShopPanelPresenter] Purchase failed (insufficient gold or already owned)");
            }
        }

        private void RefreshButtons()
        {
            var products = _shopService.GetProducts();
            var shopItems = _view.ShopItems;
            var currentGold = _goldService.Gold.Value;

            for (var i = 0; i < shopItems.Length && i < products.Count; i++)
            {
                var product = products[i];
                var isOwned = _shopService.IsPurchased(product);

                if (!isOwned)
                {
                    var canAfford = currentGold >= product.price;
                    shopItems[i].SetBuyButtonInteractable(canAfford);
                }
            }
        }

        private void OnCloseButtonClicked()
        {
            _view.Hide();
        }
    }
}
