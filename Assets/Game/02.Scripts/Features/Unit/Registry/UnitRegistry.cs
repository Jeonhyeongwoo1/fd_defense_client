using System.Collections.Generic;
using Game.Model;

namespace Game.Service
{
    public class UnitRegistry
    {
        private readonly Dictionary<UnitSide, List<UnitEntry>> _entryDict = new();

        public UnitRegistry()
        {
            _entryDict[UnitSide.Ally] = new List<UnitEntry>();
            _entryDict[UnitSide.Enemy] = new List<UnitEntry>();
        }

        public void Register(UnitEntry entry)
        {
            _entryDict[entry.Model.Side].Add(entry);
        }

        public void Unregister(UnitEntry entry)
        {
            _entryDict[entry.Model.Side].Remove(entry);
        }

        public IReadOnlyList<UnitEntry> GetEntryList(UnitSide side)
        {
            return _entryDict[side];
        }
    }
}
