using UnityEngine;
using System.Collections;

namespace MurderMystery {
    public class DoorwayScript : MonoBehaviour {

        public string targetDoor;
        public bool justUsed;
        public Vector3 center;

        // Use this for initialization
        void Start() {
            justUsed = false;

            BoxCollider2D collider = (BoxCollider2D)this.gameObject.GetComponent<Collider2D>();

            Vector2 size = collider.size;
            Vector3 centerPoint = new Vector3(collider.offset.x, collider.offset.y, 0f);
            Vector3 worldPos = transform.TransformPoint(collider.offset);
            center = worldPos;
        }

        public void Travel() {
            GameObject door = GameObject.Find(targetDoor);
            GameObject player = GameObject.Find("Player");
            player.GetComponent<Rigidbody2D>().isKinematic = true;
            player.transform.position = (Vector2)door.GetComponent<DoorwayScript>().center;
            player.GetComponent<Rigidbody2D>().isKinematic = false;
            if (player.transform.position == door.GetComponent<DoorwayScript>().center) {
                Debug.Log("player moved");
                Ceilings.makeRoomVisible(getTargetRoom());
            }            
        }

        private GameObject getTargetRoom() {
            GameObject door = GameObject.Find(targetDoor);
            GameObject roomscontainer = GameObject.Find("Rooms");
            for (int i = 0; i < roomscontainer.transform.childCount; i++) {
                Bounds doorBounds = door.GetComponent<BoxCollider2D>().bounds;
                GameObject room = roomscontainer.transform.GetChild(i).gameObject;

                if (room.GetComponent<BoxCollider2D>() != null) {
                    if (doorBounds.Intersects(room.GetComponent<BoxCollider2D>().bounds)) {
                        return room;
                    }
                }
                else {
                    if (doorBounds.Intersects(room.GetComponent<PolygonCollider2D>().bounds)) {
                        return room;
                    }
                }
            }
            return null;
        }

        /*
        void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.name == "Player" && justUsed == false) {
                GameObject door = GameObject.Find(targetDoor);
                justUsed = true;
                door.GetComponent<DoorwayScript>().justUsed = true;

                GameObject.Find("Player").transform.position = (Vector2)door.transform.position + door.GetComponent<BoxCollider2D>().offset;

                Vector2 size = door.GetComponent<BoxCollider2D>().size;
                Vector3 centerPoint = new Vector3(door.GetComponent<BoxCollider2D>().offset.x, door.GetComponent<BoxCollider2D>().offset.y, 0f);
            }
        }

        void OnTriggerExit2D(Collider2D other) {
            Debug.Log("YUP");
            if (other.gameObject.name == "Player") {
                justUsed = false;
            }
        }
        */

    }
}



