using UnityEngine;
using System.Collections;

namespace MurderMystery {
    public class Menu : MonoBehaviour {

        public void startButton() {
            Application.LoadLevel("mainv2");
        }

        public void setSeed(string val) {
            Seed.inputField(val);
        }
    }

}
