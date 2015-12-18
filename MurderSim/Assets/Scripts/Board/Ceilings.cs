using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MurderMystery {

    public static class Ceilings {
        public static GameObject[] ceilings;

        public static void findCeilings() {
            ceilings = GameObject.FindGameObjectsWithTag("Ceiling");
        }

        public static void makeRoomVisible(GameObject room) {
            foreach (GameObject ceiling in ceilings) {
                Ceiling ceilingScript = ceiling.GetComponent<Ceiling>();
                Debug.Log(ceilingScript.room + " : " + room);
                if (ceilingScript.room == room) ceilingScript.makeInvisible();
                else ceilingScript.makeVisible();
            }
        }
    }

}