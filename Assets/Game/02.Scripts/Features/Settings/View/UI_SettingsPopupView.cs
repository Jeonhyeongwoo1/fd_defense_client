using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_SettingsPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Toggle vibrationToggle;
        [SerializeField] private Button resetButton;
        [SerializeField] private GameObject resetConfirmRoot;
        [SerializeField] private Button resetConfirmButton;
        [SerializeField] private Button resetCancelButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_Text versionText;

        public GameObject Root => root;
        public Toggle VibrationToggle => vibrationToggle;
        public Button ResetButton => resetButton;
        public GameObject ResetConfirmRoot => resetConfirmRoot;
        public Button ResetConfirmButton => resetConfirmButton;
        public Button ResetCancelButton => resetCancelButton;
        public Button CloseButton => closeButton;
        public TMP_Text VersionText => versionText;

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
