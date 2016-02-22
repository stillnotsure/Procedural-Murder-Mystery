using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MurderMystery {

    public static class Ceilings {
        public static bool roomLit = false;
        public static GameObject[] ceilings;

        public static void findCeilings() {
            ceilings = GameObject.FindGameObjectsWithTag("Ceiling");
        }

        public static void makeRoomVisible(GameObject room) {
            Debug.Log("Making room visible");
            foreach (GameObject ceiling in ceilings) {
                Ceiling ceilingScript = ceiling.GetComponent<Ceiling>();
                if (ceilingScript.room == room) {
                    ceilingScript.makeInvisible();
                    roomLit = true;
                }
                else ceilingScript.makeVisible();
            }
        }
    }

}