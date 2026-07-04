using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_UnitSpawnButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image cooldownFillImage;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image unitIconImage;
        [SerializeField] private string unitId;

        public string UnitId => unitId;
        public Button Button => button;

        public void UpdateUI(string unitName, int cost)
        {
            nameText.text = unitName;
            costText.text = cost.ToString();
        }

        public void Bind(string id, string unitName, int cost, Sprite icon)
        {
            unitId = id;
            nameText.text = unitName;
            costText.text = cost.ToString();

            if (unitIconImage != null && icon != null)
            {
                unitIconImage.sprite = icon;
            }
        }

        public void SetCooldownRatio(float ratio)
        {
            cooldownFillImage.fillAmount = ratio;
        }

        public void SetInteractable(bool isInteractable)
        {
            button.interactable = isInteractable;
        }
    }
}
