using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_DailyRewardPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text[] dayTexts;
        [SerializeField] private TMP_Text[] goldTexts;
        [SerializeField] private GameObject[] claimedMarks;
        [SerializeField] private GameObject[] todayHighlights;
        [SerializeField] private Button claimButton;
        [SerializeField] private Button closeButton;

        public GameObject Root => root;
        public Button ClaimButton => claimButton;
        public Button CloseButton => closeButton;

        public void UpdateSlot(int slotIndex, int day, int gold, bool isClaimed, bool isToday)
        {
            if (slotIndex < 0 || slotIndex >= 7)
            {
                return;
            }

            dayTexts[slotIndex].text = $"Day {day}";
            goldTexts[slotIndex].text = gold.ToString();
            claimedMarks[slotIndex].SetActive(isClaimed);
            todayHighlights[slotIndex].SetActive(isToday);
        }

        public void Show()
        {
            root.SetActive(true);
        }

        public void Hide()
        {
            root.SetActive(false);
        }
    }
}
