using System.Collections.Generic;
using System.Linq;
using Game.Core;
using Game.Data;
using UnityEngine;

namespace Game.Service
{
    public class DeckService
    {
        public const int DeckSize = 10;
        private const string DeckKey = "Deck";

        private readonly UnitTableSO _unitTable;

        public DeckService(UnitTableSO unitTable)
        {
            _unitTable = unitTable;
        }

        public IReadOnlyList<string> GetDeck()
        {
            var deckStr = PlayerPrefs.GetString(DeckKey, string.Empty);

            if (string.IsNullOrEmpty(deckStr))
            {
                return GetDefaultDeck();
            }

            var unitIdArray = deckStr.Split(';');
            if (unitIdArray.Length != DeckSize)
            {
                GameLogger.LogWarning($"[DeckService] Corrupted deck data (expected {DeckSize} units, found {unitIdArray.Length}). Using default deck.");
                return GetDefaultDeck();
            }

            foreach (var unitId in unitIdArray)
            {
                var data = _unitTable.GetById(unitId);
                if (data == null)
                {
                    GameLogger.LogWarning($"[DeckService] Corrupted deck data (unit '{unitId}' not found). Using default deck.");
                    return GetDefaultDeck();
                }
            }

            return unitIdArray;
        }

        public void SaveDeck(IReadOnlyList<string> unitIdList)
        {
            if (unitIdList.Count != DeckSize)
            {
                GameLogger.LogError($"[DeckService] Cannot save deck: expected {DeckSize} units, received {unitIdList.Count}");
                return;
            }

            var deckStr = string.Join(";", unitIdList);
            PlayerPrefs.SetString(DeckKey, deckStr);
            PlayerPrefs.Save();

            GameLogger.Log($"[DeckService] Deck saved: {deckStr}");
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
                .OrderBy(u => u.cost)
                .Take(DeckSize)
                .Select(u => u.id)
                .ToList();

            return sortedUnits;
        }
    }
}
