using System;
using Game.Core;
using Game.Model;
using Game.Service;
using Game.View;
using UniRx;
using VContainer.Unity;

namespace Game.Controller
{
    public class BaseHitReactionController : IStartable, IDisposable
    {
        private readonly BaseService _baseService;
        private readonly BaseViewHolder _viewHolder;
        private readonly CompositeDisposable _disposables = new();

        public BaseHitReactionController(BaseService baseService, BaseViewHolder viewHolder)
        {
            _baseService = baseService;
            _viewHolder = viewHolder;
        }

        public void Start()
        {
            var allyBase = _baseService.GetBase(UnitSide.Ally);
            allyBase.CurrentHp
                .Pairwise()
                .Subscribe(pair =>
                {
                    if (pair.Current < pair.Previous)
                    {
                        OnBaseDamaged(UnitSide.Ally);
                    }
                })
                .AddTo(_disposables);

            var enemyBase = _baseService.GetBase(UnitSide.Enemy);
            enemyBase.CurrentHp
                .Pairwise()
                .Subscribe(pair =>
                {
                    if (pair.Current < pair.Previous)
                    {
                        OnBaseDamaged(UnitSide.Enemy);
                    }
                })
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnBaseDamaged(UnitSide side)
        {
            var view = _viewHolder.GetView(side);

            if (view != null)
            {
                view.PlayHitReaction();
            }
            else
            {
                GameLogger.LogWarning($"[BaseHitReactionController] BaseView for {side} is null");
            }
        }
    }
}
