using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MurderMystery {

    public class BoardManager : MonoBehaviour {

        private PlotGenerator pg;
        public List<Npc> npcs;

        // Use this for initialization
        void Start() {
            pg = gameObject.GetComponent<PlotGenerator>();
        }

        public void placeNPCs() {
            npcs = pg.npcs;

            foreach (Npc npc in npcs) {
                string targetRoomName = npc.currentRoom.roomName;
                GameObject targetRoom = GameObject.Find(targetRoomName);

                if (targetRoom.GetComponent<BoxCollider2D>() != null) {
                    BoxCollider2D collider = targetRoom.GetComponent<BoxCollider2D>();
                    Bounds bounds = collider.bounds;
                    float x = Random.Range(bounds.min.x, bounds.max.x);
                    float y = Random.Range(bounds.min.y, bounds.max.y);

                    npc.transform.Translate(new Vector3(x, y, 0));
                }
                else if (targetRoom.GetComponent<PolygonCollider2D>() != null) {
                    PolygonCollider2D collider = targetRoom.GetComponent<PolygonCollider2D>();
                    Bounds bounds = collider.bounds;

                    float x = 0;
                    float y = 0;

                    while (!collider.OverlapPoint(new Vector2(x, y))) {
                        x = Random.Range(bounds.min.x, bounds.max.x);
                        y = Random.Range(bounds.min.y, bounds.max.y);
                    }

                    npc.transform.Translate(new Vector3(x, y, 0));
                }
                if (!npc.isAlive) { npc.gameObject.transform.localEulerAngles = new Vector3(0, 0, 90); }
            }
        }

    }
}