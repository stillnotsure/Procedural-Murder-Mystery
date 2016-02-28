using UnityEngine;
using System.Collections;

namespace MurderMystery {
    public static class Seed {

        public static int seed;

        public static void setSeed() {
            if (seed == 0) {
                Debug.Log("Creating random seed");
                Random.seed = (int)System.DateTime.Now.Ticks;
            }
            else {
                Random.seed = seed;
            }

            seed = Random.seed;
            Debug.Log("Seed : " + seed);
        }

        public static void inputField(string val) {
            if (val != "") {
                seed = int.Parse(val);
                Debug.Log(seed);
            }
            else {
                seed = 0;
            }
        }
    }

}
