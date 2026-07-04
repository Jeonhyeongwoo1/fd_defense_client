using Game.GameState;
using UniRx;

namespace Game.Model
{
    public class GameStateModel
    {
        public IReadOnlyReactiveProperty<GameStateType> CurrentStateType => _currentStateType;

        private readonly ReactiveProperty<GameStateType> _currentStateType = new(GameStateType.Ready);

        public void SetState(GameStateType stateType)
        {
            _currentStateType.Value = stateType;
        }
    }
}
