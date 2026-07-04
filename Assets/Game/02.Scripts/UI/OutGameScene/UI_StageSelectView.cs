using System.Collections.Generic;
using UnityEngine;

namespace Game.View
{
    public class UI_StageSelectView : MonoBehaviour
    {
        [SerializeField] private UI_StageButtonView[] stageButtons;

        public IReadOnlyList<UI_StageButtonView> StageButtonList => stageButtons;
    }
}
