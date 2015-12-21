using UnityEngine;
using System.Collections.Generic;

namespace MurderMystery {

    public class Mansion : MonoBehaviour {

        public List<Room> rooms;

        void Start() {
            Room entranceHall = new Room("Entrance Hall");
            Room eastHall = (new Room("East Hall"));
            Room westHall = (new Room("West Hall"));
            Room diningRoom = (new Room("Dining Room"));
            Room study = (new Room("Study"));
            Room kitchen = (new Room("Kitchen"));
            Room livingRoom = (new Room("Living Room"));
            Room eastWashroom = (new Room("East Washroom"));
            Room westWashroom = (new Room("West Washroom"));
            Room storage = (new Room("Storage"));

            rooms.AddRange(new List<Room> { entranceHall, eastHall, westHall, diningRoom, study, kitchen, livingRoom, eastWashroom, westWashroom, storage });

            entranceHall.setNeighbouringRooms(new List<Room> { diningRoom, eastHall, westHall });
            eastHall.setNeighbouringRooms(new List<Room> { entranceHall, study, eastWashroom, livingRoom, storage });
            westHall.setNeighbouringRooms(new List<Room> { entranceHall, kitchen, westWashroom });
            diningRoom.setNeighbouringRooms(new List<Room> { entranceHall, kitchen });
            study.setNeighbouringRooms(new List<Room> { eastHall });
            kitchen.setNeighbouringRooms(new List<Room> { diningRoom, westHall });
            livingRoom.setNeighbouringRooms(new List<Room> { eastHall });
            eastWashroom.setNeighbouringRooms(new List<Room> { eastHall });
            westWashroom.setNeighbouringRooms(new List<Room> { westHall });
            storage.setNeighbouringRooms(new List<Room> { eastHall });

        }

        // Update is called once per frame
        void Update() {

        }
    }


    [System.Serializable]
    public class Room {
        public string roomName;
        public List<Npc> npcs;
        public List<GameObject> items;
        public List<Room> neighbouringRooms;
        public List<GameObject> containers;
        public GameObject ceiling;

        public Room(string roomName) {
            this.roomName = roomName;
            neighbouringRooms = new List<Room>();
            npcs = new List<Npc>();
            items = new List<GameObject>();
            containers = new List<GameObject>();
        }

        public void setNeighbouringRooms(List<Room> rooms) {
            foreach (Room room in rooms) {
                neighbouringRooms.Add(room);
            }
        }

        // Use this for initialization
        void Start() {
            npcs = new List<Npc>();
        }

        // Update is called once per frame
        void Update() {

        }
    }
}