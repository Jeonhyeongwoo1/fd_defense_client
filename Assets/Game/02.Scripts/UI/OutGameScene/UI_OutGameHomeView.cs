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
        [SerializeField] private Button settingsButton;
        [SerializeField] private GameObject stagePanelRoot;
        [SerializeField] private TMP_Text goldText;

        public Button StageTabButton => stageTabButton;
        public Button DeckTabButton => deckTabButton;
        public Button UpgradeTabButton => upgradeTabButton;
        public Button MissionsTabButton => missionsTabButton;
        public Button SettingsButton => settingsButton;
        public GameObject StagePanelRoot => stagePanelRoot;

        public void UpdateGold(int gold)
        {
            if (goldText != null)
            {
                goldText.text = gold.ToString();
            }
        }
    }
}
