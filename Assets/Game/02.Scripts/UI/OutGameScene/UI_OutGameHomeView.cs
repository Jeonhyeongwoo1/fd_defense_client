using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_OutGameHomeView : MonoBehaviour
    {
        [SerializeField] private Button stageTabButton;
        [SerializeField] private Button deckTabButton;
        [SerializeField] private Button upgradeTabButton;
        [SerializeField] private Button missionsTabButton;
        [SerializeField] private Button shopTabButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Button dailyRewardButton;
        [SerializeField] private GameObject stagePanelRoot;
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text bestStageText;
        [SerializeField] private TMP_Text unitCountText;
        [SerializeField] private TMP_Text stageNumberText;
        [SerializeField] private TMP_Text stageNameText;
        [SerializeField] private GameObject dailyRewardBadge;
        [SerializeField] private GameObject missionsBadge;

        public Button StageTabButton => stageTabButton;
        public Button DeckTabButton => deckTabButton;
        public Button UpgradeTabButton => upgradeTabButton;
        public Button MissionsTabButton => missionsTabButton;
        public Button ShopTabButton => shopTabButton;
        public Button SettingsButton => settingsButton;
        public Button PlayButton => playButton;
        public Button DailyRewardButton => dailyRewardButton;
        public GameObject StagePanelRoot => stagePanelRoot;
        public GameObject DailyRewardBadge => dailyRewardBadge;
        public GameObject MissionsBadge => missionsBadge;

        public void UpdateGold(int gold)
        {
            if (goldText != null)
            {
                goldText.text = gold.ToString();
            }
        }

        public void UpdatePlayerStats(string bestStageLabel, int ownedCount, int totalCount)
        {
            if (bestStageText != null)
            {
                bestStageText.text = bestStageLabel;
            }

            if (unitCountText != null)
            {
                unitCountText.text = $"UNITS {ownedCount}/{totalCount}";
            }
        }

        public void UpdateSelectedStage(string numberLabel, string stageName)
        {
            if (stageNumberText == null || stageNameText == null)
            {
                Game.Core.GameLogger.LogWarning("[UI_OutGameHomeView] Stage title texts not wired, skipping UpdateSelectedStage.");
                return;
            }

            stageNumberText.text = numberLabel;
            stageNameText.text = stageName;
        }
    }
}
