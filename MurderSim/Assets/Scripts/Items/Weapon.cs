using UnityEngine;
using System.Collections;
namespace MurderMystery {
    [System.Serializable]

    public class Weapon : Item {

        public enum DamageType { blunt, stab, poison, shot }
        public DamageType damageType;

        public Weapon(string name, DamageType damageType) {
            this.name = name;
            this.damageType = damageType;
        }

    }
}