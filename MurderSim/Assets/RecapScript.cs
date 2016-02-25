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
        public AudioSource audioSource;
        public GameObject lowerText;

        // Use this for initialization
        void Start() {
            pg = GameObject.Find("GameManager").GetComponent<PlotGenerator>();
            timelineText = GameObject.Find("TimelineText");

            GameObject.Find("NameText").GetComponent<Text>().text = PlotGenerator.chosenNPC.getFullName();

            StartCoroutine(RevealEvent());
        }


        // Update is called once per frame
        void Update() {

        }

        IEnumerator RevealEvent() {
            Text tlTextComponent = timelineText.GetComponent<Text>();
            List<Event> events = Timeline.fullNPCHistory(pg.murderer);
            tlTextComponent.text = "";
            yield return new WaitForSeconds(3f);

            foreach (Event e in events) {
                if (e is PickupItem) {
                    PickupItem pickup = e as PickupItem;

                    tlTextComponent.text += (String.Format("{0} : <color=red>???</color> picked up the {1} in the {2}", Timeline.convertTime(e.time), pickup.item.name, pickup.room.roomName));
                    tlTextComponent.text += System.Environment.NewLine;
                    audioSource.Play();
                    yield return new WaitForSeconds(3f);
                }

                if (e is Murder) {
                    Murder murder = e as Murder;
                    tlTextComponent.text += (String.Format("{0} : <color=red>???</color> murderered {1} in the {2}", Timeline.convertTime(e.time), murder.npc2.name, murder.room.roomName));
                    tlTextComponent.text += System.Environment.NewLine;
                    audioSource.Play();
                    yield return new WaitForSeconds(3f);
                }

                if (e is DropItem) {
                    DropItem drop = e as DropItem;
                    if (drop.item == pg.murderWeapon) {
                        tlTextComponent.text += (String.Format("{0} : <color=red>???</color> hid the {1} in the {2}", Timeline.convertTime(e.time), drop.item.name, drop.room.roomName));
                        tlTextComponent.text += System.Environment.NewLine;
                        audioSource.Play();
                        yield return new WaitForSeconds(4f);
                    }
                }
            }

            audioSource.Play();
            string s = tlTextComponent.text;
            s = Regex.Replace(s, "\\?\\?\\?", pg.murderer.getFullName());
            tlTextComponent.text = s;
            yield return new WaitForSeconds(1f);
            revealMotive();
        }

        private void revealMotive() {
            lowerText.SetActive(true);
            GameObject.Find("killerNPCText").GetComponent<Text>().text = pg.murderer.getFullName();

            Motives motive = pg.motive;
            string motiveString = "Who ";

            if (motive == Motives.inheritance) {
                motiveString += "murdered their sibling for the inheritance money";
            } 
            else if (motive == Motives.jealousLove) {
                motiveString += "murdered their rival in love";
            }
            else if (motive == Motives.loverRevenge) {
                motiveString += "murdered the lover they couldn't have";
            }
            else if (motive == Motives.revenge) {
                motiveString += "murdered their longtime enemy";
            }

            GameObject.Find("motiveText").GetComponent<Text>().text = motiveString;
           
        }

    }

    
}
