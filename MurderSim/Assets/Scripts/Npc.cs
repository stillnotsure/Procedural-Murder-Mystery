using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace MurderMystery {

    [System.Serializable]
    public class Npc : MonoBehaviour {

        public PlotGenerator pg;
        //private Mansion mansion;

        //Memories
        public List<History> histories; //Contains all the histories they know about, or are involved in

        public Dictionary<Event, Testimony> testimonies;
        public List<Testimony> fabrications;
        public int timeBuffer; //Total amount of time to push back events they're recollecting as a result of lies

        public NPCLog log;
        public enum Gender { Male, Female };
        public string firstname, surname;
        public Gender gender;

        public bool isMurderer = false;
        public bool isVictim = false;
        public bool isAlive = true;
        public bool foundBody = false;

        public List<GameObject> inventory;

        private bool hasWeapon = false;
        private bool hasMurderWeapon = false;

        [System.NonSerialized]
        public Room currentRoom;
        private Room lastRoom;

        public Family family = null;

        void Awake() {
            pg = GameObject.Find("GameManager").GetComponent<PlotGenerator>();
            histories = new List<History>();
        }
        void Start() {
            timeBuffer = 0;
            inventory = new List<GameObject>();
            testimonies = new Dictionary<Event, Testimony>();
            
        }

        void Update() {
            //Will wait with the body
            if (!pg.weaponHidden && !foundBody)
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
                            else {
                                //Todo - Brash murderers will murder even when the coast isn't clear
                                bool coastIsClear = true;
                                foreach (Npc npc in currentRoom.npcs) {
                                    if (npc != this && !npc.isVictim) coastIsClear = false;
                                }
                                if (coastIsClear) kill(pg.victim, randomWeapon());

                            }

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
            Debug.Log(new PickupItem(pg.timeSteps, this, currentRoom, item).toString());
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

        private void hideWeapon() {

            //Old version of hideWeapon() that needed the room to be empty first
            //Removed to create more interesting gameplay
            //Todo - Depending on how smart murderer is, might wait until the coast is clear to hide the weapon
            //Todo - Smart murderers might try and hide the weapon back where they found it

            if (pg.victim.currentRoom == currentRoom) { moveToRandomRoom(); }

            int hideChance = Random.Range(0, 10);
            if (hideChance > 7) {
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
                        break;
                    }
                    containers.RemoveAt(r);
                }
            } else {
                moveToRandomRoom();
            }

            /*
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
                        //Debug.Log(pg.murderWeapon + " was hidden in " + containerScript.gameObject);
                        break;
                    }
                    containers.RemoveAt(r);
                }
            }
            else {
                moveToRandomRoom();
            }
            */
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
            Timeline.addEvent(new SwitchRooms(pg.timeSteps, this, currentRoom, room));
            room.npcs.Add(this);
            currentRoom = room;

            //If there are any other NPCs, log that they were seen
            foreach (Npc npc in currentRoom.npcs) {
                if (npc != this) {
                    if (npc.isAlive) Timeline.addEvent(new Encounter(pg.timeSteps, this, npc, currentRoom));
                    else {
                        if (!isMurderer) {
                            Timeline.addEvent(new FoundBody(pg.timeSteps, this, npc, currentRoom));
                            foundBody = true;
                            pg.bodyWasFound();
                        }
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
            DropItem e = new DropItem(pg.timeSteps, this, currentRoom, item);
            Debug.Log(e.toString());
            Timeline.addEvent(new DropItem(pg.timeSteps, this, currentRoom, item));
        }

        public void addHistory(History history) {
            histories.Add(history);
            if (pg.debugMode) {
                if (history.npc1 == pg.murderer) Debug.Log(firstname + " knows the truth"); 

                Npc npc;
                if (history.npc1 == this) { npc = history.npc2; } else { npc = history.npc1; }
                Debug.Log(getFullName() + " : has a " + history.GetType() + " history with " + npc.getFullName());
            }


        }
    }

}