using UnityEngine;
using System.Collections;

namespace MurderMystery {

    //A recounting of events, as told by a specific NPC
    //These are held alongside the true events they might be describing in the NPC's memories
    //If pure fabrication they are held seperately in NPC's lies, in chronological order
    public class Testimony {
        public Event e;
        public Npc npc;
        public bool truth;

        public Testimony(Event e, Npc npc, bool truth) {
            this.e = e;
            this.npc = npc;
            this.truth = truth;
        }
    }

    public static class TestimonyManager {

        public static Testimony createTestimony(Npc npc, Event e) {
            Debug.Log("Decide to lie is running");

            if (e is SwitchRooms) {
                SwitchRooms switchrooms = e as SwitchRooms;
                if (npc.isMurderer) {
                    Debug.Log(switchrooms.time + ", time of murder " + Timeline.murderEvent.time + ", " + switchrooms.newRoom.roomName);
                    //If this would take them into the room the victim was murdered, and at the time of the murder, deny it
                    //Todo : Denying being in that room means they need to be able to figure out an alternate route to where they are standing
                    if (switchrooms.newRoom == Timeline.murderEvent.room) {
                        Debug.Log("Would have been in the killroom at the time of death, lying");
                        return lieAboutSwitchRooms(npc, switchrooms);
                    }
                }

                if (npc.timeBuffer > 0) {
                    Debug.Log("Buffering the time out");
                    SwitchRooms bufferedEvent = new SwitchRooms(e.time + npc.timeBuffer, npc, switchrooms.origRoom, switchrooms.newRoom);
                    return new Testimony(bufferedEvent, npc, false);
                }
            }


            //If no reasons to lie, return the truth
            return new Testimony(e, npc, true);
        }

        public static Testimony lieAboutSwitchRooms(Npc npc, SwitchRooms switchrooms) {
            int i = switchrooms.time;
            Room oldRoom = switchrooms.origRoom;
            Room newRoom = switchrooms.newRoom;

            //Todo - Pathfinding, so that they know how to figure out different routes to the room they're in now
            //Gonna be impossibly difficult...

            //If no alternate route, change the time and get the NPC to add the difference in truthTime and lieTime to all further switchroom testimonies
            i += Random.Range(1, 10);
            npc.timeBuffer += i;

            SwitchRooms s = new SwitchRooms(i, npc, oldRoom, newRoom);
            return new Testimony(s, npc, false);
        }
    }

}