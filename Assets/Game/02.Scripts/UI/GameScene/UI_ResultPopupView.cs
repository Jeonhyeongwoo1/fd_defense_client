using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_ResultPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button stageSelectButton;

        public Button RetryButton => retryButton;
        public Button StageSelectButton => stageSelectButton;

        public void UpdateUI(bool isVictory)
        {
            resultText.text = isVictory ? "VICTORY" : "DEFEAT";
            root.SetActive(true);
        }

        public void Hide()
        {
            root.SetActive(false);
        }
    }
}
