using UnityEngine;
using System.Collections.Generic;

namespace MurderMystery {

    [System.Serializable]
    public class Npc : MonoBehaviour {

        public PlotGenerator pg;
        public BoardManager boardManager;
        public bool checkCollisions = true;
        public BoxCollider2D boxCollider;

        //Memories
        public List<History> histories; //Contains all the histories they know about, or are involved in
        public SuspectTestimony[] chosenSuspects; //Contains up to 2 suspects, the first of which is the one told to the detective
        public Dictionary<Event, EventTestimony> testimonies;
        public Dictionary<EventTestimony, Event> testimoniesReversed;

        public List<Testimony> fabrications;
        public int timeBuffer; //Total amount of time to push back events they're recollecting as a result of lies

        public NPCLog log;
        public enum Gender { Male, Female };
        public string firstname, surname;
        public Gender gender;
        public float audioPitch;
        public int loyaltyPoint;

        public int lieAccusations = 3;
        public float stress = 0;
        public float stressIncrements = 0.3f; //How much stress increases when called out on a lie

        public bool nameKnown = false;
        public bool isMurderer = false;
        public bool isVictim = false;
        public bool isAlive = true;
        public bool foundBody = false;
        public bool moveOnNextTurn = false; //Used to quickly get away from scene of crime/hiding spot

        public List<GameObject> inventory;
        private int inventoryCapacity = 4;
        private Dictionary<string, float> actionProbabilities;

        public float pickupChance = 0.08f;
        public float putdownChance = 0.08f;
        public float meanderChance = 0.15f;
        public float nilChance = 1;


        public bool hasWeapon = false;
        private bool hasMurderWeapon = false;
    
        public Room currentRoom;
        private Room lastRoom;

        public Family family = null;

        void Awake() {
            pg = GameObject.Find("GameManager").GetComponent<PlotGenerator>();
            boardManager = GameObject.Find("GameManager").GetComponent<BoardManager>();
            histories = new List<History>();
            DontDestroyOnLoad(transform.gameObject);
            
        }

        void Start() {
            timeBuffer = 0;
            inventory = new List<GameObject>();
            testimonies = new Dictionary<Event, EventTestimony>();
            testimoniesReversed = new Dictionary<EventTestimony, Event>();
            getAudioPitch();
            stress = Random.Range(0f, 0.5f);
            loyaltyPoint = Random.Range(2, 3);
            nilChance = 1 - (pickupChance + putdownChance + meanderChance);

            actionProbabilities = new Dictionary<string, float>();
            actionProbabilities.Add("Pickup", pickupChance); actionProbabilities.Add("PutDown", putdownChance); actionProbabilities.Add("Meander", meanderChance); actionProbabilities.Add("nil", nilChance);
        }

        void Update() {
            if (isAlive)GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

            //Will wait with the body
            if ((!pg.weaponHidden || !pg.bodyFound) && !foundBody)
                act();
        }

        void OnCollisionEnter2D(Collision2D collider) {
            
            if (collider.gameObject.name != "Player") {
                if (checkCollisions) boardManager.repositionNpc(this);
            }
            
        }

        public void checkPosition() {
            string targetRoomName = currentRoom.roomName;
            GameObject targetRoom = GameObject.Find(targetRoomName);
            Collider2D roomCollider;
            if (targetRoom.GetComponent<BoxCollider2D>() != null) roomCollider = targetRoom.GetComponent<BoxCollider2D>();
            else roomCollider = targetRoom.GetComponent<PolygonCollider2D>();

            if (!roomCollider.bounds.Contains(boxCollider.bounds.min) || !roomCollider.bounds.Contains(boxCollider.bounds.max)){
                boardManager.repositionNpc(this);
            }
        }

        [ContextMenu("Reposition")]
        void reposition() {
            boardManager.repositionNpc(this);
        }

        public void act() {
            // pg.timeSteps++;
            if (isAlive) {
                //If Murderer and not finished with villanous acts
                if (isMurderer && !pg.weaponHidden) {
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


                //If not murderer, or have finished the murder and acting like a civilian
                else {
                    if (moveOnNextTurn) {
                        moveToRandomRoom();
                    } else {
                        //Random chance to take actions
                        float r = Random.Range(0f, 1f);
                        string actionChosen = null;
                        float totalWeight = 0;

                        foreach (KeyValuePair<string, float> action in actionProbabilities) {
                            totalWeight += action.Value;
                            if (r <= totalWeight) {
                                actionChosen = action.Key;
                                break;
                            }
                        }

                        switch (actionChosen) {
                            case "Meander":
                                {
                                    moveToRandomRoom();
                                    break;
                                }
                                
                            case "Pickup":
                                {
                                    PickUpRandomItem();
                                    break;
                                }
                                
                            case "PutDown":
                                {
                                    PutDownRandomItem();
                                    break;
                                }
                               
                            case "nil":
                                {
                                    break;
                                }
                                
                            default:
                                {
                                    break;
                                }
                        }
                    }
                    
                }
            }

        }

        public void addTestimony(EventTestimony testimony, Event e) {
            testimonies.Add(e, testimony);
            testimoniesReversed.Add(testimony, e);
        }

        public void removeTestimony(EventTestimony testimony = null, Event e = null) {
            if (testimony != null) testimoniesReversed.TryGetValue(testimony, out e);
            if (e != null) testimonies.TryGetValue(e, out testimony);

            testimonies.Remove(e);
            testimoniesReversed.Remove(testimony);
        }

        public string getFullName() {
            return (firstname + " " + surname);
        }

        public void PickUpRandomItem() {
            if (inventory.Count < inventoryCapacity) {
                bool foundItem = false;

                //Create list of all containers and check each randomly
                List<GameObject> containers = new List<GameObject>(currentRoom.containers);

                while (foundItem == false && containers.Count > 0) {
                    int r = Random.Range(0, containers.Count);
                    ContainerScript containerScript = containers[r].GetComponent<ContainerScript>();

                    //Create list of items and check randomly
                    List<GameObject> items = new List<GameObject>(containerScript.items);
                    while (containerScript.items.Count > 0 && foundItem == false) {

                        int i = Random.Range(0, items.Count);

                        //NPCs don't interfere with the murder weapon
                        if (containerScript.items[i] != pg.murderWeapon) {
                            foundItem = true;
                            pickupItemFromContainer(items[i].GetComponent<Item>(), containerScript);
                            break;
                        }

                        items.RemoveAt(i);
                    }
                    containers.RemoveAt(r);
                }
            }
            
        }

        public void PutDownRandomItem() {
            if (inventory.Count > 0) {
                bool putdownItem = false;
                int i = Random.Range(0, inventory.Count); GameObject item = inventory[i];

                //Create list of all containers and check each randomly
                List<GameObject> containers = new List<GameObject>(currentRoom.containers);

                while (putdownItem == false && containers.Count > 0) {
                    int r = Random.Range(0, containers.Count);
                    ContainerScript containerScript = containers[r].GetComponent<ContainerScript>();

                    if (containerScript.capacity > containerScript.items.Count) {
                        placeItemInContainer(item.GetComponent<Item>(), containerScript);
                        break;
                    }
                    containers.RemoveAt(r);
                }
            }
        }

        private void pickupItem(Item item) {

            if (item.room != null) {
                item.room.items.Remove(item.gameObject);
                item.room = null;
            }
            
            item.setState(Item.ItemState.held);

            inventory.Add(item.gameObject);

            PickupItem e = new PickupItem(pg.timeSteps, this, currentRoom, item);
            if (pg.debugMode) Debug.Log(e.toString());
            Timeline.addEvent(e);

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
            Debug.Log("seeking vicitm");
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
                        moveOnNextTurn = true;
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
            Event e = new Murder(pg.timeSteps, this, npc, currentRoom, weapon);
            Timeline.addEvent(new Murder(pg.timeSteps, this, npc, currentRoom, weapon));
            npc.die();
            pg.murderWeapon = weapon;
            hasMurderWeapon = true;
            stress = Mathf.Min(stress + Random.Range(0.1f, 0.3f), 1.0f);

            if (pg.debugMode) Debug.Log(e.toString());
        }

        public void die() {
            isAlive = false;
            GetComponent<BoxCollider2D>().isTrigger = false;
            GetComponent<SpriteRenderer>().sortingOrder = 0;
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
                        if (!isMurderer && !pg.bodyFound) {
                            Event e = new FoundBody(pg.timeSteps, this, npc, currentRoom);
                            Timeline.addEvent(e);

                            if (pg.debugMode) Debug.Log(e.toString());

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
            if (pg.debugMode)   Debug.Log(e.toString());
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

        private void getAudioPitch() {
            if (gender == Gender.Male) {
                audioPitch = Random.Range(0.3f, 0.7f);
            } else
                audioPitch = Random.Range(0.8f, 1.5f);
        }
    }

}