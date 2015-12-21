using UnityEngine;
using System.Collections.Generic;

namespace MurderMystery {

    [System.Serializable]
    public class Npc : MonoBehaviour {

        public PlotGenerator pg;
        //private Mansion mansion;

        public NPCLog log;
        public enum Gender { Male, Female };
        public string firstname, surname;
        public Gender gender;

        public bool isMurderer = false;
        public bool isVictim = false;
        public bool isAlive = true;

        public List<GameObject> inventory;

        private bool hasWeapon = false;
        private bool hasMurderWeapon = false;

        [System.NonSerialized]
        public Room currentRoom;
        private Room lastRoom;

        public Family family = null;

        void Start() {
            Debug.Log(this.ToString() + " started");
            inventory = new List<GameObject>();
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

            inventory.Add(item.gameObject);

            Timeline.addEvent(new PickupItem(pg.timeSteps, this, currentRoom, item));
        }

        private void pickupItemFromContainer(Item item, ContainerScript container) {
            container.items.Remove(item.gameObject);
            pickupItem(item);
        }

        private void dropItem(Item item) {
            inventory.Remove(item.gameObject);
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

            //Create list of all containers and check each randomly
            List<GameObject> containers = new List<GameObject>(currentRoom.containers);

            while (foundWeapon == false && containers.Count > 0) {
                int r = Random.Range(0, containers.Count);
                ContainerScript containerScript = containers[r].GetComponent<ContainerScript>();

                //Create list of items and check randomly
                List<GameObject> items = new List<GameObject>(containerScript.items);
                while (containerScript.items.Count > 0 && foundWeapon == false) {

                    int i = Random.Range(0, items.Count);
                    Item itemscript = items[i].GetComponent<Item>();
                    Debug.Log("Weapon found - " + itemscript.name + " : " + containers[r].name);

                    if (itemscript is Weapon) {
                        foundWeapon = true;
                        pickupItemFromContainer(items[i].GetComponent<Item>(), containerScript);
                        hasWeapon = true;
                        break;
                    }

                    items.RemoveAt(i);
                }
                containers.RemoveAt(r);
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

            if (roomEmpty) {
                bool hidWeapon = false;
                //Create list of all containers and check each randomly
                List<GameObject> containers = new List<GameObject>(currentRoom.containers);

                while (hidWeapon == false && containers.Count > 0) {
                    int r = Random.Range(0, containers.Count);
                    ContainerScript containerScript = containers[r].GetComponent<ContainerScript>();

                    if (containerScript.capacity > containerScript.items.Count) {
                        placeItemInContainer(pg.murderWeapon, containerScript);
                        hasMurderWeapon = false;
                        pg.weaponWasHidden();
                        Debug.Log(pg.murderWeapon + " was hidden in " + containerScript.gameObject);
                        break;
                    }
                    containers.RemoveAt(r);
                }
            }
            else {
                moveToRandomRoom();
            }
        }

        public void kill(Npc npc, Weapon weapon) {
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

            foreach (GameObject item in inventory) {
                Item itemscript = item.GetComponent<Item>();
                if (itemscript is Weapon)
                    weapons.Add((Weapon)itemscript);
            }

            int r = Random.Range(0, weapons.Count);
            return weapons[r];
        }

        private void placeItemInContainer(Item item, ContainerScript container) {
            container.addItem(item.gameObject);
            item.setState(Item.ItemState.contained);
            inventory.Remove(item.gameObject);
        }
    }
}