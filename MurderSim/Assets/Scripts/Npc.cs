using UnityEngine;
using System.Collections;

[System.Serializable]
public class Npc {

    public enum Gender { Male, Female };

    public string firstname, surname;
    public Gender gender;

    [System.NonSerialized]
    public Family family;

    public Npc(){
        family = null;
    }
}
