using UnityEngine;
using System.Collections;

namespace MurderMystery {

    [System.Serializable]
    public class Item : MonoBehaviour {

        public enum ItemState { held, dropped, contained, onDisplay }

        public ItemState state;
        public Room room;

        public void setState(ItemState state) {
            this.state = state;
            if (state == ItemState.held || state == ItemState.contained) {
                gameObject.GetComponent<Renderer>().enabled = false;
            }
            else {
                gameObject.GetComponent<Renderer>().enabled = true;
            }
        }

    }



}
