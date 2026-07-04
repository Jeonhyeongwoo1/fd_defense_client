using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_DeckPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private UI_UnitCardView[] unitCards;
        [SerializeField] private TMP_Text deckCountText;
        [SerializeField] private Button confirmButton;

        public GameObject Root => root;
        public UI_UnitCardView[] UnitCards => unitCards;
        public Button ConfirmButton => confirmButton;

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

        public void UpdateDeckCount(int current, int max)
        {
            if (deckCountText != null)
            {
                deckCountText.text = $"{current}/{max}";
            }
        }
    }
}
