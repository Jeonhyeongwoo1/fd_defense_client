using Game.Model;
using UnityEngine;

namespace Game.View
{
    public class BaseViewHolder : MonoBehaviour
    {
        [SerializeField] private BaseView allyBaseView;
        [SerializeField] private BaseView enemyBaseView;

        public BaseView GetView(UnitSide side)
        {
            return side == UnitSide.Ally ? allyBaseView : enemyBaseView;
        }
    }
}
