using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Npc : MonoBehaviour{

    public PlotGenerator pg;
    //private Mansion mansion;

    public NPCLog log;
    public enum Gender { Male, Female };
    public string firstname, surname;
    public Gender gender;

    public bool isMurderer = false;
    public bool isVictim = false;
    public bool isAlive = true;

    public List<Item> inventory;
    private bool hasWeapon = false;

    [System.NonSerialized]
    public Room currentRoom;
    private Room lastRoom;

    [System.NonSerialized]
    public Family family = null;

    void Start() {
        inventory = new List<Item>();
        pg = GameObject.Find("GameManager").GetComponent<PlotGenerator>();
        //mansion = GameObject.Find("GameManager").GetComponent<Mansion>();

        InvokeRepeating("act", 1.5f, 0.9f);
    }

    void Update() {
    }

    public void act() {
        //If Murderer
        if (isMurderer) {
            if (pg.victim.GetComponent<Npc>().isAlive) {
                if (hasWeapon) {
                    log.NewActivity("Looking for " + pg.victim.GetComponent<Npc>().firstname);
                    if (!victimInRoom()) seekVictim();
                    else kill(pg.victim, randomWeapon());

                } else {
                    log.NewActivity("Looking for a weapon");
                    seekWeapon();
                }
            }
        }

        //If not murderer
    }

    private void pickupItem(Item item) {
        item.room.items.Remove(item);
        item.room = null;
        item.held = true;
        inventory.Add(item);
        
        log.NewActivity("Picked up " + item.name);
    }

    private void dropItem(Item item) {
        inventory.Remove(item);
        item.held = false;
        item.room = currentRoom;
        currentRoom.items.Add(item);
    }

    private bool victimInRoom() {
        bool foundVictim = false;
        foreach (Npc npc in currentRoom.npcs) {
            if (npc.isVictim) {
                foundVictim = true;
                break;
            }
        }
        return foundVictim;
    }

    private void seekVictim() {
        //TODO - low - pathfinding for victim search

        List<Room> neighbouringRooms = new List<Room>(currentRoom.neighbouringRooms);
        if (lastRoom != null && neighbouringRooms.Count > 1) neighbouringRooms.Remove(lastRoom);   //Makes it so they won't check the room they were just in

        int r = Random.Range(0, neighbouringRooms.Count);
        enterRoom(neighbouringRooms[r]);
    }

    private void seekWeapon() {
        bool foundWeapon = false;

        foreach (Item item in currentRoom.items) {
            if (item is Weapon) {
                foundWeapon = true;
                pickupItem(item);
                hasWeapon = true;
                break;
            }
        }

        if (!foundWeapon) {
            //Select a room to move into randomly
            //TODO - v. low - pathfinding for weapon search
            int r = Random.Range(0, currentRoom.neighbouringRooms.Count);
            enterRoom(currentRoom.neighbouringRooms[r]);
        }

    }

    //TODO - high - Create permanent event for killings, including time, location, weapon
    public void kill(Npc npc, Weapon weapon){
        log.NewActivity("Killed " + npc.firstname + " " + npc.surname + " with a " + weapon.name);
        npc.die();
        SoundManager.instance.PlayMusic();
    }

    public void die() {
        isAlive = false;
    }

    public void enterRoom(Room room) {
        if (currentRoom != null) {
            currentRoom.npcs.Remove(this);
            lastRoom = currentRoom;
        }
        room.npcs.Add(this);
        currentRoom = room;
        log.NewActivity("Moved to " + currentRoom.roomName);
    }

    public Weapon randomWeapon() {
        List<Weapon> weapons = new List<Weapon>();

        foreach (Item item in inventory) {
            if (item is Weapon)
                weapons.Add((Weapon)item);
        }

        int r = Random.Range(0, weapons.Count);
        return weapons[r];
    }

}
