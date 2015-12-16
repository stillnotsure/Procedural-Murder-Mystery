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
    private bool hasMurderWeapon = false;

    public Room currentRoom;
    private Room lastRoom;

    [System.NonSerialized]
    public Family family = null;

    void Start() {
        inventory = new List<Item>();
        pg = GameObject.Find("GameManager").GetComponent<PlotGenerator>();
    }

    void Update() {
        if (!pg.weaponHidden)
            act();
    }

    public void act() {
       // pg.timeSteps++;

        if (isAlive) {
            //If Murderer
            if (isMurderer) {
                if (pg.victim.GetComponent<Npc>().isAlive) {
                    if (hasWeapon) {
                        if (!victimInRoom()) seekVictim();
                        else kill(pg.victim, randomWeapon());

                    }
                    else {
                        seekWeapon();
                    }
                }
                else {
                    //If already killed the victim
                    if (hasMurderWeapon) {
                        hideWeapon();
                    }
                }
            }

            //If not murderer
            else {
                //Random chance to meander
                float r = Random.Range(0f, 1f);

                if (r > 0.8f) {
                    moveToRandomRoom();
                }
            }
        }

    }

    public string getFullName() {
        return (firstname + " " + surname);
    }

    private void pickupItem(Item item) {
        item.room.items.Remove(item.gameObject);
        item.room = null;
        item.setState(Item.ItemState.held);
        inventory.Add(item);
        
        Timeline.addEvent(new PickupItem(pg.timeSteps, this, currentRoom, item));
    }

    private void dropItem(Item item) {
        inventory.Remove(item);
        item.setState(Item.ItemState.dropped);
        item.room = currentRoom;
        currentRoom.items.Add(item.gameObject);
        if (item = pg.murderWeapon) {
            hasMurderWeapon = false;
            pg.weaponWasHidden();
        }
            

        Timeline.addEvent(new DropItem(pg.timeSteps, this, currentRoom, item));
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

        foreach (GameObject item in currentRoom.items) {
            if (item.GetComponent<Item>() is Weapon) {
                foundWeapon = true;
                pickupItem(item.GetComponent<Item>());
                hasWeapon = true;
                break;
            }
        }

        if (!foundWeapon) {
            //TODO - v. low - pathfinding for weapon search
            moveToRandomRoom();
        }

    }

    //Conditions for hiding a weapon so far are only that there is no one to see it happen
    private void hideWeapon() {
        bool roomEmpty = true;
        foreach (Npc npc in currentRoom.npcs) {
            if (npc != this) roomEmpty = false;
        }
        //TODO - High - Hide weapon in containers etc. rather than just dropping
        if (roomEmpty) {
            dropItem(pg.murderWeapon);
        }
        else {
            moveToRandomRoom();
        }
    }

    public void kill(Npc npc, Weapon weapon){
        Timeline.addEvent(new Murder(pg.timeSteps, this, npc, currentRoom, weapon));
        npc.die();
        pg.murderWeapon = weapon;
        hasMurderWeapon = true;
        SoundManager.instance.PlayMusic();
    }

    public void die() {
        isAlive = false;
    }

    private void moveToRandomRoom() {
        int r = Random.Range(0, currentRoom.neighbouringRooms.Count);
        enterRoom(currentRoom.neighbouringRooms[r]);
    }

    public void enterRoom(Room room) {
        if (currentRoom != null) {
            currentRoom.npcs.Remove(this);
            lastRoom = currentRoom;
        }
        room.npcs.Add(this);
        Timeline.addEvent(new SwitchRooms(pg.timeSteps, this, currentRoom, room));
        currentRoom = room;

        //If there are any other NPCs, log that they were seen
        foreach (Npc npc in currentRoom.npcs) {
            if (npc != this) {
                if (npc.isAlive) Timeline.addEvent(new Encounter(pg.timeSteps, this, npc, currentRoom));
                else {
                    Timeline.addEvent(new FoundBody(pg.timeSteps, this, npc, currentRoom));
                    pg.bodyWasFound();
                }
            }
        }
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
