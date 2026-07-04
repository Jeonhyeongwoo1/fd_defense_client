using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_PausePopupView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button stageSelectButton;

        public Button PauseButton => pauseButton;
        public Button ResumeButton => resumeButton;
        public Button RetryButton => retryButton;
        public Button StageSelectButton => stageSelectButton;

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
    }
}
