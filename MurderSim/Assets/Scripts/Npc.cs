using UnityEngine;
using System.Collections;

[System.Serializable]
public class Npc {

    private PlotGenerator pg;
    public NPCLog log;
    public enum Gender { Male, Female };
    public string firstname, surname;
    public Gender gender;

    public bool isMurderer;
    public bool isVictim;
    public bool isAlive;

    [System.NonSerialized]
    public Room currentRoom;

    [System.NonSerialized]
    public Family family;

    void Start() {
       
    }
    public Npc(){
        family = null;
        isMurderer = false;
        isVictim = false;
        isAlive = true;
    }

    public void initLog() {
        log.NewActivity("First test");

        if (family != null) { log.NewActivity("Part of the " + family.family_name + " family"); }
    }

    public void act() {


    }

    public void kill(Npc npc){
        log.NewActivity("Killed " + npc.firstname + " " + npc.surname);
        npc.die();
    }

    public void die() {
        isAlive = false;
    }

    public void enterRoom(Room room) {
        if (currentRoom != null) {
            currentRoom.npcs.Remove(this);
        }
        room.npcs.Add(this);
        currentRoom = room;
    }

}
