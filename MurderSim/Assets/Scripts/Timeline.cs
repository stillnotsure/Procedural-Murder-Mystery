using UnityEngine;
using System.Collections.Generic;
using System;

namespace MurderMystery {
    public static class Timeline {

        public static List<Event> events = new List<Event>();
        public static DateTime startTime = Convert.ToDateTime("17:00");
        public static int timeIncrements = 5; //minutes
        public static Murder murderEvent;

        public static void addEvent(Event e) {
            events.Add(e);
        }

        public static Event getLast() {
            return events[events.Count - 1];
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

        public static void printTimeline() {
            foreach (Event e in events) {
                Debug.Log(e.toString());
            }
        }

        public static List<Event> locationDuringTimeframe(Npc npc, int timeStep1, int timeStep2) {
            List<Event> alibi = new List<Event>();

            foreach (Event e in events) {
                if (e is SwitchRooms && e.npc == npc && e.time >= timeStep1 && e.time <= timeStep2) {
                    alibi.Add(e);
                }
            }

            return alibi;
        }

        public static List<Event> EventsWitnessedDuringTimeframe(Npc npc, int timeStep1, int timeStep2) {
            List<Event> eventsWitnessed = new List<Event>();

            foreach (Event e in events) {
                if (e is FoundBody && e.npc == npc) {
                    eventsWitnessed.Add(e);
                }
                if (e.npc != npc && e.time >= timeStep1 && e.time <= timeStep2) {

                    foreach (Npc witness in e.witnesses) {
                        if (witness == npc) {
                            eventsWitnessed.Add(e);
                        }
                    }

                }
            }

            return eventsWitnessed;
        }

        //Takes a timestep and returns the datetime in relation to what time the game's events begin
        public static string convertTime(int timeStep) {
            return String.Format("{0:t}", startTime.AddMinutes(timeStep * timeIncrements));
        }

    }

    public interface Event {

        int time { get; set; }
        Npc npc { get; set; }
        List<Npc> witnesses { get; set; }
        string toString();

    }

    public class SwitchRooms : Event {

        public int time { get; set; }
        public Npc npc { get; set; }
        public List<Npc> witnesses { get; set; }
        public Room origRoom;
        public Room newRoom;
        public List<Npc> peopleInNewRoom;

        public SwitchRooms(int time, Npc npc, Room origRoom, Room newRoom) {
            this.time = time; this.npc = npc; this.origRoom = origRoom; this.newRoom = newRoom;
            witnesses = new List<Npc>(origRoom.npcs);
            peopleInNewRoom = new List<Npc>(newRoom.npcs);
            witnesses.AddRange(newRoom.npcs);
        }

        public string toString() {
            return String.Format("{3} : {0} moved from {1} to {2} ", npc.name, origRoom.roomName, newRoom.roomName, Timeline.convertTime(time));
        }
    }

    public class FoundBody : Event {

        public int time { get; set; }
        public Npc npc { get; set; }
        public List<Npc> witnesses { get; set; }
        public Npc body;
        public Room room;

        public FoundBody(int time, Npc npc, Npc body, Room room) {
            this.time = time; this.npc = npc; this.body = body; this.room = room;
            witnesses = new List<Npc>(room.npcs);
        }

        public string toString() {
            return String.Format("{3} : {0} found {1}'s corpse in {2} ", npc.firstname, body.firstname, room.roomName, Timeline.convertTime(time));
        }
    }

    public class Encounter : Event {

        public int time { get; set; }
        public Npc npc { get; set; }
        public List<Npc> witnesses { get; set; }
        public Npc npc2;
        public Room room;

        public Encounter(int time, Npc npc, Npc npc2, Room room) {
            this.time = time; this.npc = npc; this.npc2 = npc2; this.room = room;
            witnesses = new List<Npc>(room.npcs);
        }

        public string toString() {
            return String.Format("{3} : {0} encountered {1} in {2} ", npc.firstname, npc2.firstname, room.roomName, Timeline.convertTime(time));
        }
    }

    public class Murder : Event {

        public int time { get; set; }
        public Npc npc { get; set; }
        public List<Npc> witnesses { get; set; }
        public Npc npc2;
        public Room room;
        public Weapon weapon;

        public Murder(int time, Npc npc, Npc npc2, Room room, Weapon weapon) {
            this.time = time; this.npc = npc; this.npc2 = npc2; this.room = room; this.weapon = weapon;
            witnesses = new List<Npc>(room.npcs);
            Timeline.murderEvent = this;
        }

        public string toString() {
            return String.Format("{4} : {0} murderered {1} in {2} using a {3} ", npc.firstname, npc2.firstname, room.roomName, weapon.name, Timeline.convertTime(time));
        }
    }

    public class PickupItem : Event {

        public int time { get; set; }
        public Npc npc { get; set; }
        public List<Npc> witnesses { get; set; }
        public Room room;
        public Item item;

        public PickupItem(int time, Npc npc, Room room, Item item) {
            this.time = time; this.npc = npc; this.room = room; this.item = item;
            witnesses = new List<Npc>(room.npcs);
        }

        public string toString() {
            return String.Format("{3} : {0} picked up {1} in {2}", npc.firstname, item.name, room.roomName, Timeline.convertTime(time));
        }

    }

    public class DropItem : Event {

        public int time { get; set; }
        public Npc npc { get; set; }
        public List<Npc> witnesses { get; set; }
        public Room room;
        public Item item;

        public DropItem(int time, Npc npc, Room room, Item item) {
            this.time = time; this.npc = npc; this.room = room; this.item = item;
            witnesses = new List<Npc>(room.npcs);
        }

        public string toString() {
            return String.Format("{3} : {0} dropped {1} in {2}", npc.firstname, item.name, room.roomName, Timeline.convertTime(time));
        }

    }


}