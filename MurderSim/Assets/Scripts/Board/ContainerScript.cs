using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace MurderMystery {

    public class ContainerScript : MonoBehaviour {

        public List<GameObject> items;
        public Room room;
        public string roomName;

        void Start() {
            items = new List<GameObject>();
            Mansion mansion = GameObject.Find("GameManager").GetComponent<Mansion>();
            
            //TODO - Fix this! This is horrible and ineffecient. Room scripts should be added to the gameObject as it's imported from Tiled, then the import script can simply gameobject.find(roomname) to add containers toi t
            foreach(Room room in mansion.rooms) {
                if (room.roomName == roomName) {
                    room.containers.Add(gameObject);
                    this.room = room;
                }
            }

        }

        public void addItem(GameObject item) {
            items.Add(item);
            item.GetComponent<Item>().setState(Item.ItemState.contained);
        }

        public void removeItem(GameObject item) {
            items.Remove(item);
            item.GetComponent<Item>().setState(Item.ItemState.held);
        }
    }

}

