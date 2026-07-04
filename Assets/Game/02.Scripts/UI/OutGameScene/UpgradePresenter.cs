using System;
using Game.Core;
using Game.Data;
using Game.Service;
using Game.View;
using UniRx;
using VContainer.Unity;

namespace Game.Presenter
{
    public class UpgradePresenter : IStartable, IDisposable
    {
        private readonly UI_UpgradePanelView _view;
        private readonly UpgradeService _upgradeService;
        private readonly GoldService _goldService;
        private readonly UnitTableSO _unitTable;
        private readonly CompositeDisposable _disposables = new();

        private string _selectedUnitId;

        public UpgradePresenter(
            UI_UpgradePanelView view,
            UpgradeService upgradeService,
            GoldService goldService,
            UnitTableSO unitTable)
        {
            _view = view;
            _upgradeService = upgradeService;
            _goldService = goldService;
            _unitTable = unitTable;
        }

        public void Start()
        {
            var cards = _view.UnitCards;
            var allUnits = _unitTable.UnitDataList;

            if (cards.Length < allUnits.Count)
            {
                GameLogger.LogWarning($"[UpgradePresenter] Card count ({cards.Length}) < unit count ({allUnits.Count})");
            }

            for (var i = 0; i < cards.Length && i < allUnits.Count; i++)
            {
                var unitData = allUnits[i];
                var level = _upgradeService.GetLevel(unitData.id);

                cards[i].Bind(unitData.id, unitData.iconSprite, unitData.unitName, level, false);

                var card = cards[i];
                card.Button.onClick.AsObservable()
                    .Subscribe(_ => OnCardClicked(card))
                    .AddTo(_disposables);
            }

            _view.UpgradeButton.onClick.AsObservable()
                .Subscribe(_ => OnUpgradeClicked())
                .AddTo(_disposables);

            _goldService.Gold.Subscribe(_ => RefreshUpgradeButton()).AddTo(_disposables);
        }

        public void Refresh()
        {
            var cards = _view.UnitCards;
            var allUnits = _unitTable.UnitDataList;

            for (var i = 0; i < cards.Length && i < allUnits.Count; i++)
            {
                var unitData = allUnits[i];
                var level = _upgradeService.GetLevel(unitData.id);
                var isSelected = unitData.id == _selectedUnitId;

                cards[i].Bind(unitData.id, unitData.iconSprite, unitData.unitName, level, isSelected);
            }

            if (!string.IsNullOrEmpty(_selectedUnitId))
            {
                RefreshDetail();
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnCardClicked(UI_UnitCardView card)
        {
            var previousSelection = _selectedUnitId;
            _selectedUnitId = card.UnitId;

            var cards = _view.UnitCards;
            foreach (var c in cards)
            {
                c.SetSelected(c.UnitId == _selectedUnitId);
            }

            RefreshDetail();
        }

        private void OnUpgradeClicked()
        {
            if (string.IsNullOrEmpty(_selectedUnitId))
            {
                GameLogger.LogWarning("[UpgradePresenter] No unit selected for upgrade");
                return;
            }

            var success = _upgradeService.TryUpgrade(_selectedUnitId);

            if (success)
            {
                RefreshDetail();
                RefreshCardLevel(_selectedUnitId);
            }
            else
            {
                GameLogger.Log("[UpgradePresenter] Upgrade failed (insufficient gold or max level)");
            }
        }

        private void RefreshDetail()
        {
            if (string.IsNullOrEmpty(_selectedUnitId))
            {
                return;
            }

            var unitData = _unitTable.GetById(_selectedUnitId);
            if (unitData == null)
            {
                GameLogger.LogError($"[UpgradePresenter] UnitData not found: {_selectedUnitId}");
                return;
            }

            var currentLevel = _upgradeService.GetLevel(_selectedUnitId);
            var isMaxLevel = _upgradeService.IsMaxLevel(_selectedUnitId);

            var hpMultiplier = _upgradeService.GetHpMultiplier(_selectedUnitId);
            var attackMultiplier = _upgradeService.GetAttackMultiplier(_selectedUnitId);

            var currentHp = (int)(unitData.hp * hpMultiplier);
            var currentAttack = (int)(unitData.attackPower * attackMultiplier);

            var nextHpMultiplier = _upgradeService.GetNextHpMultiplier(_selectedUnitId);
            var nextAttackMultiplier = _upgradeService.GetNextAttackMultiplier(_selectedUnitId);

            var nextHp = (int)(unitData.hp * nextHpMultiplier);
            var nextAttack = (int)(unitData.attackPower * nextAttackMultiplier);

            var upgradeCost = _upgradeService.GetUpgradeCost(_selectedUnitId);

            _view.UpdateDetail(
                unitData.unitName,
                currentLevel,
                currentHp,
                nextHp,
                currentAttack,
                nextAttack,
                upgradeCost,
                isMaxLevel
            );

            RefreshUpgradeButton();
        }

        private void RefreshUpgradeButton()
        {
            if (string.IsNullOrEmpty(_selectedUnitId))
            {
                return;
            }

            var isMaxLevel = _upgradeService.IsMaxLevel(_selectedUnitId);
            if (isMaxLevel)
            {
                return;
            }

            var cost = _upgradeService.GetUpgradeCost(_selectedUnitId);
            var canAfford = _goldService.Gold.Value >= cost;

            _view.UpgradeButton.interactable = canAfford;
        }

        private void RefreshCardLevel(string unitId)
        {
            var cards = _view.UnitCards;
            foreach (var card in cards)
            {
                if (card.UnitId == unitId)
                {
                    var unitData = _unitTable.GetById(unitId);
                    if (unitData != null)
                    {
                        var level = _upgradeService.GetLevel(unitId);
                        card.Bind(unitId, unitData.iconSprite, unitData.unitName, level, true);
                    }
                    break;
                }
            }
        }
    }
}
