using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_UpgradePanelView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private UI_UnitCardView[] unitCards;
        [SerializeField] private TMP_Text detailNameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text hpText;
        [SerializeField] private TMP_Text attackText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private GameObject maxLevelMark;
        [SerializeField] private Button closeButton;

        public GameObject Root => root;
        public UI_UnitCardView[] UnitCards => unitCards;
        public Button UpgradeButton => upgradeButton;
        public Button CloseButton => closeButton;

        public void Show()
        {
            if (root != null)
            {
                root.SetActive(true);
            }
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }

        public void UpdateDetail(string unitName, int level, int currentHp, int nextHp, int currentAttack, int nextAttack, int upgradeCost, bool isMaxLevel)
        {
            if (detailNameText != null)
            {
                detailNameText.text = unitName;
            }

            if (levelText != null)
            {
                levelText.text = $"Lv.{level}";
            }

            if (hpText != null)
            {
                hpText.text = isMaxLevel ? $"{currentHp}" : $"{currentHp} -> {nextHp}";
            }

            if (attackText != null)
            {
                attackText.text = isMaxLevel ? $"{currentAttack}" : $"{currentAttack} -> {nextAttack}";
            }

            if (costText != null)
            {
                costText.text = upgradeCost.ToString();
            }

            if (upgradeButton != null)
            {
                upgradeButton.gameObject.SetActive(!isMaxLevel);
            }

            if (maxLevelMark != null)
            {
                maxLevelMark.SetActive(isMaxLevel);
            }
        }
    }
}
