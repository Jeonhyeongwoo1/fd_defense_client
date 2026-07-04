using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_StageButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text stageNameText;
        [SerializeField] private GameObject clearedMark;
        [SerializeField] private GameObject lockedOverlay;
        [SerializeField] private string stageId;

        public string StageId => stageId;
        public Button Button => button;

        public void UpdateUI(string displayName, bool isCleared, bool isUnlocked)
        {
            stageNameText.text = displayName;
            clearedMark.SetActive(isCleared);
            lockedOverlay.SetActive(!isUnlocked);
            button.interactable = isUnlocked;
        }
    }
}
