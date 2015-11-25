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

    void Update() {
        //TODO - Mid : Set this to toggle rather than updating it every frame for no reason
        if (held) gameObject.GetComponent<Renderer>().enabled = false;
        else gameObject.GetComponent<Renderer>().enabled = true;
    }
}
