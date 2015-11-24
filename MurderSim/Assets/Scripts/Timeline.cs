using UnityEngine;
using System.Collections.Generic;
using System;

public interface Event {

    Time time { get; set; }

    string toString();

}

public class SwitchRooms : Event {

    public Time time { get; set; }
    public Npc npc;
    public Room origRoom;
    public Room newRoom;

    public SwitchRooms(Time time, Npc npc, Room origRoom, Room newRoom) {
        this.time = time; this.npc = npc; this.origRoom = origRoom; this.newRoom = newRoom;
    }

    public string toString() {
        return String.Format("{0} moved from {1} to {2} ", npc, origRoom, newRoom);
    }
}

public class Encounter : Event {

    public Time time { get; set; }
    public Npc npc1;
    public Npc npc2;
    public Room room;

    public Encounter(Time time, Npc npc1, Npc npc2, Room room) {
        this.time = time; this.npc1 = npc1; this.npc2 = npc2; this.room = room;
    }

    public string toString() {
        return String.Format("{0} encountered {1} in {2} ", npc1, npc2, room);
    }
}

public class Murder : Event {

    public Time time { get; set; }
    public Npc npc1;
    public Npc npc2;
    public Room room;
    public Weapon weapon;

    public Murder(Time time, Npc npc1, Npc npc2, Room room, Weapon weapon) {
        this.time = time; this.npc1 = npc1; this.npc2 = npc2; this.room = room; this.weapon = weapon;
    }

    public string toString() {
        return String.Format("{0} murdererd {1} in {2} using a {3} ", npc1, npc2, room, weapon);
    }
}

public class Timeline : MonoBehaviour {

    public List<Event> events;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void addEvent(Event e){
        events.Add(e);
    }
}
