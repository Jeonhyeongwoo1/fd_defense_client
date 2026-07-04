using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_MissionRowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private Button claimButton;
        [SerializeField] private GameObject claimedMark;

        public Button ClaimButton => claimButton;

        public void Bind(string description, int current, int target, bool isCompleted, bool isClaimed)
        {
            descriptionText.text = description;
            progressText.text = $"{current}/{target}";

            if (isClaimed)
            {
                claimButton.gameObject.SetActive(false);
                claimedMark.SetActive(true);
                progressText.gameObject.SetActive(false);
            }
            else if (isCompleted)
            {
                claimButton.gameObject.SetActive(true);
                claimButton.interactable = true;
                claimedMark.SetActive(false);
                progressText.gameObject.SetActive(true);
            }
            else
            {
                claimButton.gameObject.SetActive(true);
                claimButton.interactable = false;
                claimedMark.SetActive(false);
                progressText.gameObject.SetActive(true);
            }
        }
    }
}
