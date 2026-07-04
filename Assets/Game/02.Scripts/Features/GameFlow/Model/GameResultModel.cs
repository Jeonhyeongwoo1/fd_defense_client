using Game.Model;

namespace Game.Model
{
    public class GameResultModel
    {
        public UnitSide? Winner { get; private set; }
        public int EarnedGold { get; private set; }

        public void SetWinner(UnitSide side)
        {
            Winner = side;
        }

        public void SetEarnedGold(int gold)
        {
            EarnedGold = gold;
        }
    }
}
