using UnityEngine;
using System.Collections;

[System.Serializable]
public class Weapon : Item {

    public enum DamageType { blunt, stab, poison, shot }
    public DamageType damageType;

    public Weapon(string name, DamageType damageType) {
        this.name = name;
        this.damageType = damageType;
    }

}
