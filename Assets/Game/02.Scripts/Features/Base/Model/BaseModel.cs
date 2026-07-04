using Game.Model;
using UniRx;
using UnityEngine;

namespace Game.Model
{
    public class BaseModel
    {
        public UnitSide Side { get; }
        public int MaxHp { get; private set; }
        public IReadOnlyReactiveProperty<int> CurrentHp => _currentHp;
        public bool IsDestroyed => _currentHp.Value <= 0;

        private readonly ReactiveProperty<int> _currentHp = new();

        public BaseModel(UnitSide side)
        {
            Side = side;
        }

        public void Initialize(int maxHp)
        {
            MaxHp = maxHp;
            _currentHp.Value = maxHp;
        }

        public void ApplyDamage(int damage)
        {
            _currentHp.Value = Mathf.Max(0, _currentHp.Value - damage);
        }
    }
}
