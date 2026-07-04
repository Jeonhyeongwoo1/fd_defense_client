using System;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class BossData
    {
        public string id;
        public string unitName;
        public int hp;
        public int attackPower;
        public float attackInterval;
        public float attackRange;
        public float moveSpeed;
        public int skillDamage;
        public float skillInterval;
        public float skillRange;
        public GameObject prefab;
    }
}
