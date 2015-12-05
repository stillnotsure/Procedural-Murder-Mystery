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
            GameObject.Find("Player").GetComponent<Rigidbody2D>().isKinematic = true;
            GameObject.Find("Player").transform.position = (Vector2)door.GetComponent<DoorwayScript>().center;
            GameObject.Find("Player").GetComponent<Rigidbody2D>().isKinematic = false;
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



