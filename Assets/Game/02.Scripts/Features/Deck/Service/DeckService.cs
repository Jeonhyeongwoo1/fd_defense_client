using System.Collections.Generic;
using System.Linq;
using Game.Core;
using Game.Data;

namespace Game.Service
{
    public class DeckService
    {
        public const int DeckSize = 10;

        private readonly UnitTableSO _unitTable;
        private readonly UserDataService _userDataService;

        public DeckService(UnitTableSO unitTable, UserDataService userDataService)
        {
            _unitTable = unitTable;
            _userDataService = userDataService;
        }

        public bool IsUnitOwned(string unitId)
        {
            return _userDataService.IsUnitOwned(unitId);
        }

        public IReadOnlyList<string> GetDeck()
        {
            var deckUnitIds = _userDataService.Data.deckUnitIds;

            if (deckUnitIds.Count == 0)
            {
                return GetDefaultDeck();
            }

            if (deckUnitIds.Count != DeckSize)
            {
                GameLogger.LogWarning($"[DeckService] Corrupted deck data (expected {DeckSize} units, found {deckUnitIds.Count}). Using default deck.");
                return GetDefaultDeck();
            }

            foreach (var unitId in deckUnitIds)
            {
                var data = _unitTable.GetById(unitId);
                if (data == null)
                {
                    GameLogger.LogWarning($"[DeckService] Corrupted deck data (unit '{unitId}' not found). Using default deck.");
                    return GetDefaultDeck();
                }

                if (!IsUnitOwned(unitId))
                {
                    GameLogger.LogWarning($"[DeckService] Corrupted deck data (unit '{unitId}' not owned). Using default deck.");
                    return GetDefaultDeck();
                }
            }

            return deckUnitIds;
        }

        public void SaveDeck(IReadOnlyList<string> unitIdList)
        {
            if (unitIdList.Count != DeckSize)
            {
                GameLogger.LogError($"[DeckService] Cannot save deck: expected {DeckSize} units, received {unitIdList.Count}");
                return;
            }

            _userDataService.Data.deckUnitIds.Clear();
            foreach (var unitId in unitIdList)
            {
                _userDataService.Data.deckUnitIds.Add(unitId);
            }
            _userDataService.Save();

            GameLogger.Log($"[DeckService] Deck saved: {string.Join(";", unitIdList)}");
        }

        public bool IsInDeck(string unitId)
        {
            var deck = GetDeck();
            foreach (var id in deck)
            {
                if (id == unitId)
                {
                    return true;
                }
            }
            return false;
        }

        private IReadOnlyList<string> GetDefaultDeck()
        {
            var sortedUnits = _unitTable.UnitDataList
                .Where(u => IsUnitOwned(u.id))
                .OrderBy(u => u.cost)
                .Take(DeckSize)
                .Select(u => u.id)
                .ToList();

            return sortedUnits;
        }
    }
}
