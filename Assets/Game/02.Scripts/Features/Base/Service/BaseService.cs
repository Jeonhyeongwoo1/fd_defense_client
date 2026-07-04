using Game.Data;
using Game.Model;

namespace Game.Service
{
    public class BaseService
    {
        private readonly BaseModel _allyBase;
        private readonly BaseModel _enemyBase;

        public BaseService()
        {
            _allyBase = new BaseModel(UnitSide.Ally);
            _enemyBase = new BaseModel(UnitSide.Enemy);
        }

        public void Initialize(StageData data)
        {
            _allyBase.Initialize(data.allyBaseHp);
            _enemyBase.Initialize(data.enemyBaseHp);
        }

        public void ApplyDamage(UnitSide targetSide, int damage)
        {
            var targetBase = GetBase(targetSide);

            if (targetBase.IsDestroyed)
            {
                return;
            }

            targetBase.ApplyDamage(damage);
        }

        public BaseModel GetBase(UnitSide side)
        {
            return side == UnitSide.Ally ? _allyBase : _enemyBase;
        }

        public bool IsBaseDestroyed(UnitSide side)
        {
            return GetBase(side).IsDestroyed;
        }
    }
}
