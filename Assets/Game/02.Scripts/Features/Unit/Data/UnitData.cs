using System;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class UnitData
    {
        public string id;
        public string unitName;
        public int hp;
        public int attackPower;
        public float attackInterval;
        public float attackRange;
        public float moveSpeed;
        public int cost;
        public float cooldown;
        public GameObject prefab;
    }
}
