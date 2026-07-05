using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_MissionPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private UI_MissionRowView[] missionRows;
        [SerializeField] private Button closeButton;

        public GameObject Root => root;
        public UI_MissionRowView[] MissionRows => missionRows;
        public Button CloseButton => closeButton;

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
