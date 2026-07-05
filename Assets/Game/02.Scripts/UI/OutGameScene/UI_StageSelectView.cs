using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_StageSelectView : MonoBehaviour
    {
        public IReadOnlyList<UI_StageButtonView> StageButtonList => stageButtons;
        public Button CloseButton => closeButton;

        [SerializeField] private UI_StageButtonView[] stageButtons;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject root;

        public void Hide()
        {
            root.SetActive(false);
        }
    }
}
