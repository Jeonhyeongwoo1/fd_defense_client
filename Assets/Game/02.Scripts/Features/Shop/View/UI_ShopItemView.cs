using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_ShopItemView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Button buyButton;
        [SerializeField] private GameObject ownedMark;

        public Button BuyButton => buyButton;

        public void Bind(Sprite icon, string unitName, int price, bool isOwned)
        {
            if (iconImage != null && icon != null)
            {
                iconImage.sprite = icon;
            }

            if (nameText != null)
            {
                nameText.text = unitName;
            }

            if (priceText != null)
            {
                priceText.text = price.ToString();
            }

            if (buyButton != null)
            {
                buyButton.gameObject.SetActive(!isOwned);
            }

            if (ownedMark != null)
            {
                ownedMark.SetActive(isOwned);
            }
        }

        public void SetBuyButtonInteractable(bool interactable)
        {
            if (buyButton != null)
            {
                buyButton.interactable = interactable;
            }
        }
    }
}
