using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_ResultPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private GameObject winRoot;
        [SerializeField] private GameObject loseRoot;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button stageSelectButton;

        public Button RetryButton => retryButton;
        public Button StageSelectButton => stageSelectButton;

        public void UpdateUI(bool isVictory)
        {
            if (winRoot != null)
            {
                winRoot.SetActive(isVictory);
            }

            if (loseRoot != null)
            {
                loseRoot.SetActive(!isVictory);
            }

            root.SetActive(true);
        }

        public void Hide()
        {
            root.SetActive(false);
        }
    }
}
