using UnityEngine;
using System.Collections;

[System.Serializable]
public class Npc {

    public enum Gender { Male, Female };
    public Family family;
    public string firstname, surname;
    public Gender gender;

    public Npc(){
        family = null;
    }
}
