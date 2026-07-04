using System;
using System.Collections.Generic;
using Game.Core;
using Game.Data;
using Game.Service;
using Game.View;
using UniRx;
using VContainer.Unity;

namespace Game.Presenter
{
    public class DeckPresenter : IStartable, IDisposable
    {
        private readonly UI_DeckPanelView _view;
        private readonly DeckService _deckService;
        private readonly UnitTableSO _unitTable;
        private readonly UpgradeService _upgradeService;
        private readonly CompositeDisposable _disposables = new();

        private List<string> _tempSelectedUnitIdList = new();

        public DeckPresenter(
            UI_DeckPanelView view,
            DeckService deckService,
            UnitTableSO unitTable,
            UpgradeService upgradeService)
        {
            _view = view;
            _deckService = deckService;
            _unitTable = unitTable;
            _upgradeService = upgradeService;
        }

        public void Start()
        {
            var currentDeck = _deckService.GetDeck();
            _tempSelectedUnitIdList = new List<string>(currentDeck);

            var cards = _view.UnitCards;
            var allUnits = _unitTable.UnitDataList;

            if (cards.Length < allUnits.Count)
            {
                GameLogger.LogWarning($"[DeckPresenter] Card count ({cards.Length}) < unit count ({allUnits.Count})");
            }

            for (var i = 0; i < cards.Length && i < allUnits.Count; i++)
            {
                var unitData = allUnits[i];
                var level = _upgradeService.GetLevel(unitData.id);
                var isSelected = _tempSelectedUnitIdList.Contains(unitData.id);

                cards[i].Bind(unitData.id, unitData.iconSprite, unitData.unitName, level, isSelected);

                var card = cards[i];
                card.Button.onClick.AsObservable()
                    .Subscribe(_ => OnCardClicked(card))
                    .AddTo(_disposables);
            }

            _view.UpdateDeckCount(_tempSelectedUnitIdList.Count, DeckService.DeckSize);

            _view.ConfirmButton.onClick.AsObservable()
                .Subscribe(_ => OnConfirmClicked())
                .AddTo(_disposables);
        }

        public void Refresh()
        {
            var currentDeck = _deckService.GetDeck();
            _tempSelectedUnitIdList = new List<string>(currentDeck);

            var cards = _view.UnitCards;
            var allUnits = _unitTable.UnitDataList;

            for (var i = 0; i < cards.Length && i < allUnits.Count; i++)
            {
                var unitData = allUnits[i];
                var level = _upgradeService.GetLevel(unitData.id);
                var isSelected = _tempSelectedUnitIdList.Contains(unitData.id);

                cards[i].Bind(unitData.id, unitData.iconSprite, unitData.unitName, level, isSelected);
            }

            _view.UpdateDeckCount(_tempSelectedUnitIdList.Count, DeckService.DeckSize);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnCardClicked(UI_UnitCardView card)
        {
            var unitId = card.UnitId;
            var isCurrentlySelected = _tempSelectedUnitIdList.Contains(unitId);

            if (isCurrentlySelected)
            {
                _tempSelectedUnitIdList.Remove(unitId);
                card.SetSelected(false);
            }
            else
            {
                if (_tempSelectedUnitIdList.Count >= DeckService.DeckSize)
                {
                    GameLogger.Log($"[DeckPresenter] Deck is full ({DeckService.DeckSize})");
                    return;
                }

                _tempSelectedUnitIdList.Add(unitId);
                card.SetSelected(true);
            }

            _view.UpdateDeckCount(_tempSelectedUnitIdList.Count, DeckService.DeckSize);
        }

        private void OnConfirmClicked()
        {
            if (_tempSelectedUnitIdList.Count != DeckService.DeckSize)
            {
                GameLogger.LogWarning($"[DeckPresenter] Deck not full ({_tempSelectedUnitIdList.Count}/{DeckService.DeckSize})");
                return;
            }

            _deckService.SaveDeck(_tempSelectedUnitIdList);
            GameLogger.Log("[DeckPresenter] Deck saved successfully");
        }
    }
}
