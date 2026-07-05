using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_ShopPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private UI_ShopItemView[] shopItems;
        [SerializeField] private Button closeButton;

        public UI_ShopItemView[] ShopItems => shopItems;
        public Button CloseButton => closeButton;

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
