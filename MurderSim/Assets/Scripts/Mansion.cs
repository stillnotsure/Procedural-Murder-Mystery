using UnityEngine;
using System.Collections.Generic;

public class Mansion : MonoBehaviour {

    public List<Room> rooms;
	
    public void setupRooms() {
        Room entranceHall = new Room("Entrance Hall");
        Room eastHall = (new Room("East Hall"));
        Room westHall = (new Room("West Hall"));
        Room diningRoom = (new Room("Dining Room"));
        Room study = (new Room("Study"));
        Room kitchen = (new Room("Kitchen"));
        Room livingRoom = (new Room("Living Room"));
        Room eastWashroom = (new Room("East Washroom"));
        Room westWashroom = (new Room("West Washroom"));
        Room sewingRoom = (new Room("Sewing Room"));
        Room utilityCloset = (new Room("Utility Closet"));

        rooms.AddRange(new List<Room> { entranceHall, eastHall, westHall, diningRoom, study, kitchen, livingRoom, eastWashroom, westWashroom, sewingRoom, utilityCloset});

        entranceHall.setNeighbouringRooms(new List<Room> {diningRoom, eastHall, westHall });
        eastHall.setNeighbouringRooms(new List<Room> { entranceHall, study, eastWashroom, livingRoom, utilityCloset});
        westHall.setNeighbouringRooms(new List<Room> {entranceHall, sewingRoom, kitchen, westWashroom });
        diningRoom.setNeighbouringRooms(new List<Room> {entranceHall, kitchen });
        study.setNeighbouringRooms(new List<Room> {eastHall});
        kitchen.setNeighbouringRooms(new List<Room> {diningRoom, westHall});
        livingRoom.setNeighbouringRooms(new List<Room> {eastHall});
        eastWashroom.setNeighbouringRooms(new List<Room> {eastHall });
        westWashroom.setNeighbouringRooms(new List<Room> {westHall});
        sewingRoom.setNeighbouringRooms(new List<Room> {westHall});
        utilityCloset.setNeighbouringRooms(new List<Room> {eastHall});

    }

	// Update is called once per frame
	void Update () {
	
	}
}


[System.Serializable]
public class Room {     
    public string roomName;
    public List<Npc> npcs;
    public List<Item> items;
    public List<Room> neighbouringRooms;

    public Room(string roomName) {
        this.roomName = roomName;
        neighbouringRooms = new List<Room>();
        npcs = new List<Npc>();
        items = new List<Item>();
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
