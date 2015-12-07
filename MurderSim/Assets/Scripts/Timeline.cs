using UnityEngine;
using System.Collections.Generic;
using System;

public static class Timeline {

    public static List<Event> events;

    public static void addEvent(Event e) {
        events = new List<Event>();
        events.Add(e);
        printLast();
    }

    public static void printLast() {
        Debug.Log(events[events.Count - 1].toString());
    }

    public static List<Event> fullNPCHistory(Npc npc) {
        List<Event> history = new List<Event>();

        foreach (Event e in events) {
            if (e.npc == npc) {
                history.Add(e);
            }
        }

        return history;
    }

}

public interface Event {

    float time { get; set; }
    Npc npc { get; set; }

    string toString();

}

public class SwitchRooms : Event {

    public float time { get; set; }
    public Npc npc { get; set; }
    public Room origRoom;
    public Room newRoom;

    public SwitchRooms(float time, Npc npc, Room origRoom, Room newRoom) {
        this.time = time; this.npc = npc; this.origRoom = origRoom; this.newRoom = newRoom;
    }

    public string toString() {
        return String.Format("{0} moved from {1} to {2} ", npc.name, origRoom.roomName, newRoom.roomName);
    }
}

public class FoundBody : Event {

    public float time { get; set; }
    public Npc npc { get; set; }
    public Npc body;
    public Room room;

    public FoundBody(float time, Npc npc, Npc body, Room room) {
        this.time = time; this.npc = npc; this.body = body; this.room = room;
    }

    public string toString() {
        return String.Format("{0} found {1}'s corpse in {2} ", npc.firstname, body.firstname, room.roomName);
    }
}

public class Encounter : Event {

    public float time { get; set; }
    public Npc npc { get; set; }
    public Npc npc2;
    public Room room;

    public Encounter(float time, Npc npc, Npc npc2, Room room) {
        this.time = time; this.npc = npc; this.npc2 = npc2; this.room = room;
    }

    public string toString() {
        return String.Format("{0} encountered {1} in {2} ", npc.firstname, npc2.firstname, room.roomName);
    }
}

public class Murder : Event {

    public float time { get; set; }
    public Npc npc { get; set; }
    public Npc npc2;
    public Room room;
    public Weapon weapon;

    public Murder(float time, Npc npc, Npc npc2, Room room, Weapon weapon) {
        this.time = time; this.npc = npc; this.npc2 = npc2; this.room = room; this.weapon = weapon;
    }

    public string toString() {
        return String.Format("{0} murderered {1} in {2} using a {3} ", npc.firstname, npc2.firstname, room.roomName, weapon.name);
    }
}

public class PickupItem : Event {

    public float time { get; set; }
    public Npc npc { get; set; }
    public Room room;
    public Item item;

    public PickupItem(float time, Npc npc, Room room, Item item) {
        this.time = time; this.npc = npc; this.room = room; this.item = item;
    }

    public string toString() {
        return String.Format("{0} picked up {1} in {2}", npc.firstname, item.name, room.roomName);
    }

}

public class DropItem : Event {

    public float time { get; set; }
    public Npc npc { get; set; }
    public Room room;
    public Item item;

    public DropItem(float time, Npc npc, Room room, Item item) {
        this.time = time; this.npc = npc; this.room = room; this.item = item;
    }

    public string toString() {
        return String.Format("{0} dropped {1} in {2}", npc.firstname, item.name, room.roomName);
    }

}


