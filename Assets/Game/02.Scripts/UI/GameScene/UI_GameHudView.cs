using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_GameHudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text waveText;
        [SerializeField] private Image allyBaseHpFillImage;
        [SerializeField] private Image enemyBaseHpFillImage;
        [SerializeField] private UI_UnitSpawnButtonView[] spawnButtons;

        public IReadOnlyList<UI_UnitSpawnButtonView> SpawnButtonList => spawnButtons;

        public void UpdateMoney(int money)
        {
            moneyText.text = money.ToString();
        }

        public void UpdateWave(int currentWave, int totalWave)
        {
            waveText.text = $"Wave {currentWave}/{totalWave}";
        }

        public void UpdateBossWave()
        {
            waveText.text = "BOSS WAVE";
        }

        public void UpdateAllyBaseHp(float ratio)
        {
            allyBaseHpFillImage.fillAmount = ratio;
        }

        public void UpdateEnemyBaseHp(float ratio)
        {
            enemyBaseHpFillImage.fillAmount = ratio;
        }
    }
}
