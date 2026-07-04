using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class UI_UnitCardView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private GameObject selectedMark;
        [SerializeField] private GameObject dimOverlay;
        [SerializeField] private string unitId;

        public string UnitId => unitId;
        public Button Button => button;

        public void Bind(string id, Sprite icon, string unitName, int level, bool isSelected)
        {
            unitId = id;

            if (icon != null && iconImage != null)
            {
                iconImage.sprite = icon;
            }

            if (nameText != null)
            {
                nameText.text = unitName;
            }

            if (levelText != null)
            {
                levelText.text = $"Lv.{level}";
            }

            SetSelected(isSelected);
        }

        public void SetSelected(bool isSelected)
        {
            if (selectedMark != null)
            {
                selectedMark.SetActive(isSelected);
            }
        }

        public void SetDim(bool isDimmed)
        {
            if (dimOverlay != null)
            {
                dimOverlay.SetActive(isDimmed);
            }
        }
    }
}
