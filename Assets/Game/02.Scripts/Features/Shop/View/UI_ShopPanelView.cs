using UnityEngine;

namespace Game.View
{
    public class UI_ShopPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private UI_ShopItemView[] shopItems;

        public UI_ShopItemView[] ShopItems => shopItems;

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
