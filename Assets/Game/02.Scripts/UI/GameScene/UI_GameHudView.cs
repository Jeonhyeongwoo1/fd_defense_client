using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.View
{
    public class UI_GameHudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private UI_UnitSpawnButtonView[] spawnButtons;

        public IReadOnlyList<UI_UnitSpawnButtonView> SpawnButtonList => spawnButtons;

        public void UpdateMoney(int money)
        {
            moneyText.text = money.ToString();
        }
    }
}
