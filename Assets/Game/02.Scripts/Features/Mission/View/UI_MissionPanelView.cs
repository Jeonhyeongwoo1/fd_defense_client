using UnityEngine;

namespace Game.View
{
    public class UI_MissionPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private UI_MissionRowView[] missionRows;

        public GameObject Root => root;
        public UI_MissionRowView[] MissionRows => missionRows;

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
