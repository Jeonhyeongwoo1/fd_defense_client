using System.Collections.Generic;
using Game.Core;
using Game.Data;
using Game.Service;
using UnityEngine;
using VContainer.Unity;

namespace Game.Service
{
    public class LobbyShowcaseService : IStartable
    {
        private const float ShowcaseX = -2f;

        private readonly DeckService _deckService;
        private readonly UnitTableSO _unitTable;

        public LobbyShowcaseService(DeckService deckService, UnitTableSO unitTable)
        {
            _deckService = deckService;
            _unitTable = unitTable;
        }

        public void Start()
        {
            var deck = _deckService.GetDeck();
            if (deck == null || deck.Count == 0)
            {
                GameLogger.LogWarning("[LobbyShowcaseService] Deck is empty. Skipping showcase.");
                return;
            }

            var firstUnitId = deck[0];
            var unitData = _unitTable.GetById(firstUnitId);

            if (unitData == null)
            {
                GameLogger.LogWarning($"[LobbyShowcaseService] UnitData not found for {firstUnitId}. Skipping showcase.");
                return;
            }

            if (unitData.prefab == null)
            {
                GameLogger.LogWarning($"[LobbyShowcaseService] Unit {firstUnitId} has no prefab. Skipping showcase.");
                return;
            }

            var position = new Vector3(ShowcaseX, Const.GroundY, 0f);
            var instance = Object.Instantiate(unitData.prefab);
            instance.transform.position = position;

            var animator = instance.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.Play("Idle");
            }

            var spriteRenderer = instance.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = false;
            }

            GameLogger.Log($"[LobbyShowcaseService] Showcased unit: {firstUnitId}");
        }
    }
}
