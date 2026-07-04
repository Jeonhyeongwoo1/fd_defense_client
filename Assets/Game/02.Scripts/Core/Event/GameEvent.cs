namespace Game.Core
{
    // All event structs must be declared in this file only.

    public struct EnemyKilledEvent
    {
        public bool IsBoss;
    }

    public struct StageClearedEvent
    {
        public string StageId;
    }

    public struct UnitUpgradedEvent
    {
        public string UnitId;
    }

    public struct UnitUnlockedEvent
    {
        public string UnitId;
    }
}
