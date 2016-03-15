using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MurderMystery {

    //A recounting of events, as told by a specific NPC
    //These are held alongside the true events they might be describing in the NPC's memories
    //If pure fabrication they are held seperately in NPC's lies, in chronological order
    public class Testimony {
        public Npc npc;
        public bool truth;
        public bool omitted; //Omitted events are those that the NPC decided not to tell, as such are still considered untrue even though they are based on real events
        public bool firstTimeTold = true; //First time lies are told, NPC stress increases
    }

    public class EventTestimony : Testimony{
        public Event e;

        public EventTestimony(Event e, Npc npc, bool truth, bool omitted) {
            this.e = e;
            this.npc = npc;
            this.truth = truth;
            this.omitted = omitted;
        }
    }

    //A suspect put forward by an NPC
    //Each NPC may only have one suspect
    //The suspect chosen may be a result of the NPC knowing a history, or could be a lie to suit their needs
    public class SuspectTestimony : Testimony{
        public History motive;

        public SuspectTestimony(Npc npc, bool truth, bool omitted, History motive) {
            this.npc = npc;
            this.truth = truth;
            this.omitted = omitted;
            this.motive = motive;
        }
    }

    public static class TestimonyManager {

        public static PlotGenerator pg;

        public static EventTestimony getTrueEventTestimony(Npc npc, Event e) {
            //Removes the existing false testimony and returns the truth
            npc.removeTestimony(e: e);
            return new EventTestimony(e, npc, true, false);
        }

        //Based on the type of event, runs through different scenarios in which an NPC might lie. If none apply it returns the truth in a new EventTestimony
        public static EventTestimony createTestimony(Npc npc, Event e) {

            if (e is Murder) {
                Murder murder = e as Murder;

                //If referring to self or someone they love...
                if (e.npc == npc || pg.relationships[pg.npcs.IndexOf(npc), pg.npcs.IndexOf(e.npc)] == 3) {
                    //Definitely don't tell the detective they witnessed the murder!
                    return new EventTestimony(e, npc, false, true);
                }
            }

            if (e is SwitchRooms) {
                SwitchRooms switchrooms = e as SwitchRooms;

                if (npc.isMurderer || npc.paranoid) {     
                    //If this would take them into the room the victim was murdered, and at the time of the murder, deny it
                    if (switchrooms.newRoom == Timeline.murderEvent.room && (switchrooms.time > Timeline.murderEvent.time - 2 && switchrooms.time < Timeline.murderEvent.time + 2)) {
                        if (pg.debugMode) Debug.Log(Timeline.convertTime(switchrooms.time) + ", time of murder " + Timeline.convertTime(Timeline.murderEvent.time) + ", " + switchrooms.newRoom.roomName);
                        return lieAboutSwitchRooms(npc, switchrooms);
                    }
                }

                if (npc.timeBuffer > 0) {
                    SwitchRooms bufferedEvent = new SwitchRooms(e.time + npc.timeBuffer, npc, switchrooms.origRoom, switchrooms.newRoom);
                    Debug.Log("Buffering the time out for " + bufferedEvent.newRoom.roomName);
                    return new EventTestimony(bufferedEvent, npc, false, false);
                }
            }

            if (e is PickupItem) {
                PickupItem pickupItem = e as PickupItem;
                Item item = pickupItem.item;

                //If referring to self or someone they love...
                //Todo - Make the required relationship value be based on their loyalty/personality
                if (e.npc == npc || pg.relationships[pg.npcs.IndexOf(npc), pg.npcs.IndexOf(e.npc)] >= npc.loyaltyPoint) {
                    //If it's the murder weapon, don't tell the detective they ever picked it up

                        Debug.Log("ommitting");
                        return new EventTestimony(e, npc, false, true);
                    
                }

            }

            if (e is DropItem) {
                DropItem dropItem = e as DropItem;
                Item item = dropItem.item;

                //If referring to self or someone they love...
                //Todo - Make the required relationship value be based on their loyalty/personality
                if (e.npc == npc || pg.relationships[pg.npcs.IndexOf(npc), pg.npcs.IndexOf(e.npc)] >= npc.loyaltyPoint) {
                    //If it's the murder weapon, don't tell the detective they ever dropped it
                    if (item == pg.murderWeapon) {
                        return new EventTestimony(e, npc, false, true);
                    }
                }
            }

            //No reason to lie about finding the body
            if (e is FoundBody) {
                return new EventTestimony(e, npc, true, false);
            }

            //If no reasons to lie, return the truth
            return new EventTestimony(e, npc, true, false);
        }

        public static EventTestimony lieAboutSwitchRooms(Npc npc, SwitchRooms switchrooms) {
            int i = switchrooms.time;
            Room oldRoom = switchrooms.origRoom;
            Room newRoom = switchrooms.newRoom;

            //change the time and get the NPC to add the difference in truthTime and lieTime to all further switchroom testimonies

            /*
            i += Random.Range(1, 3);
            npc.timeBuffer += i;
            */

            npc.timeBuffer += ( (Timeline.murderEvent.time  + pg.timeOfDeathLeeway) - switchrooms.time);

            SwitchRooms s = new SwitchRooms(i + npc.timeBuffer, npc, oldRoom, newRoom);
            return new EventTestimony(s, npc, false, false);
        }

        /*
        public static SuspectTestimony[] pickSuspects(Npc npc) {
            Npc victim = pg.victim;
            SuspectTestimony testimony1; SuspectTestimony testimony2;
            List<History> historyCopy = new List<History>(npc.histories);

            while (historyCopy.Count > 0) {
                int r = Random.Range(0, historyCopy.Count);
                History history = historyCopy[r];

                if 
            }

        }
        */

        public static SuspectTestimony pickASuspect(Npc npc) {
            List<SuspectTestimony> possibleSuspects = new List<SuspectTestimony>();
            Npc victim = pg.victim;

            //TODO - Able to lie about suspects

            //Check if there's anyone who it makes sense to blame
            foreach (History history in npc.histories) {

                //Don't tell the detective about history involving self
                if (history.npc1 != npc && history.npc2 != npc) {
                    if (history.whichNpcIsVictim == 0) {
                        if (history.npc1 == victim || history.npc2 == victim) {
                            Npc other;
                            if (history.npc1 == victim) { other = history.npc2; }
                            else { other = history.npc1; }

                            if (pg.relationships[pg.npcs.IndexOf(npc), pg.npcs.IndexOf(other)] >= npc.loyaltyPoint)
                                possibleSuspects.Add(new SuspectTestimony(other, true, true, history));
                            else
                                possibleSuspects.Add(new SuspectTestimony(other, true, false, history));
                        }

                    }
                    else if (history.whichNpcIsVictim == 1) {
                        if (history.npc2 == victim) {
                            if (pg.relationships[pg.npcs.IndexOf(npc), pg.npcs.IndexOf(history.npc1)] >= npc.loyaltyPoint)
                                possibleSuspects.Add(new SuspectTestimony(history.npc1, true, true, history));
                            else
                                possibleSuspects.Add(new SuspectTestimony(history.npc1, true, false, history));
                        }
                    }

                    else if (history.whichNpcIsVictim == 2) {
                        if (history.npc1 == victim) {
                            if (pg.relationships[pg.npcs.IndexOf(npc), pg.npcs.IndexOf(history.npc2)] >= npc.loyaltyPoint)
                                possibleSuspects.Add(new SuspectTestimony(history.npc2, true, true, history));
                            else
                                possibleSuspects.Add(new SuspectTestimony(history.npc2, true, false, history));
                        }
                    }

                }
                

            }

            //If not going to lie, randomly choose one of these motives
            if (possibleSuspects.Count > 0) {
                int r = Random.Range(0, possibleSuspects.Count);
                return possibleSuspects[r];
            } else {
                return null;
            }
                  
        }

    }

}