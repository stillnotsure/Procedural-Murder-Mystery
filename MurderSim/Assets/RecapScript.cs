using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace MurderMystery {
    public class RecapScript : MonoBehaviour {

        GameObject timelineText;
        PlotGenerator pg;

        // Use this for initialization
        void Start() {
            pg = GameObject.Find("GameManager").GetComponent<PlotGenerator>();
            timelineText = GameObject.Find("TimelineText");
            Text tlTextComponent = timelineText.GetComponent<Text>();
            List<Event> events = Timeline.fullNPCHistory(pg.murderer);
            tlTextComponent.text = "";

            foreach (Event e in events) {
                if (e is PickupItem) {
                    PickupItem pickup = e as PickupItem;

                    tlTextComponent.text += (String.Format("{0} : <color=red>???</color> picked up the {1} in the {2}",Timeline.convertTime(e.time), pickup.item.name, pickup.room.roomName));
                    tlTextComponent.text += System.Environment.NewLine;
                }
                    
                if (e is Murder) {
                    Murder murder = e as Murder;
                    tlTextComponent.text += (String.Format("{0} : <color=red>???</color> murdererd {1} in the {2}", Timeline.convertTime(e.time), murder.npc2.name, murder.room.roomName));
                    tlTextComponent.text += System.Environment.NewLine;
                }

                if (e is DropItem) {
                    DropItem drop = e as DropItem;
                    if (drop.item == pg.murderWeapon) {
                        tlTextComponent.text += (String.Format("{0} : <color=red>???</color> hid the {1} in the {2}", Timeline.convertTime(e.time), drop.item.name, drop.room.roomName));
                        tlTextComponent.text += System.Environment.NewLine;
                        break;
                    }
                }
                
            }

            string s = tlTextComponent.text;
            s = Regex.Replace(s, "\\?\\?\\?", pg.murderer.getFullName());
            tlTextComponent.text = s;

        }


        // Update is called once per frame
        void Update() {

        }
    }

}
