using UnityEngine;
using System.Collections;

namespace MurderMystery {
    public class playerControl : MonoBehaviour {

        //References
        public PlayerAnimation animation;
        public GameObject currentRoom;
        public ConversationScript conversationScript;
        public InventoryManager inventoryManager;
        public GameObject room;

        //Movement Vars
        public float playerSpeed = 2.0f;
        public BoxCollider2D box;
        private string lastDirection;

        public GameObject facing;

        // Use this for initialization
        void Start() {

            //references
            animation = GetComponent<PlayerAnimation>();
            conversationScript = GameObject.Find("GameManager").GetComponent<ConversationScript>();
            inventoryManager = GameObject.Find("GameManager").GetComponent<InventoryManager>();

            box = GetComponent<BoxCollider2D>();
            lastDirection = "up";
            facing = null;
            
            
        }

        void FixedUpdate() {
            GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
            if (!Ceilings.roomLit) Ceilings.makeRoomVisible(currentRoom);
            checkCollisions();
            if (conversationScript.state == conversationState.none && inventoryManager.state == inventoryState.none)
                Move();
            else
                animation.stopMove();
        }

        void Move() {
            float horiz = Input.GetAxisRaw("Horizontal");
            float vertic = Input.GetAxisRaw("Vertical");

            transform.Translate(horiz * 5 * Time.deltaTime, vertic * 5 * Time.deltaTime, 0);
            
            if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow)) { lastDirection = "up"; animation.faceUp(); animation.move(); }
            else if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow)) { lastDirection = "down"; animation.faceDown(); animation.move(); }
            else if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow)) { lastDirection = "left"; animation.faceLeft(); animation.move(); }
            else if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow)) { lastDirection = "right"; animation.faceRight(); animation.move(); }
            else animation.stopMove();
        }

        void checkCollisions() {
            Vector2 origin = transform.position;
            Vector2 direction = origin;
            switch (lastDirection) {
                case "up":
                    direction = Vector2.up;
                    break;
                case "right":
                    direction = Vector2.right;
                    break;
                case "down":
                    direction = Vector2.down;
                    break;
                case "left":
                    direction = Vector2.left;
                    break;
            }

            Ray ray = new Ray(origin, direction);
            int layerMask1 = 1 << LayerMask.NameToLayer("NPCs");
            int layerMask2 = 1 << LayerMask.NameToLayer("Containers");
            int layerMaskCombined = layerMask1 | layerMask2;

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1.0f, layerMaskCombined);
            //Debug.DrawRay(ray.origin, ray.direction, Color.green, 1.0f, true); //Maybe set to false later


            if (hit) {
                facing = hit.transform.gameObject;
                if (hit.transform.CompareTag("NPC")) {

                    //If player isn't already in a conversation and presses shift
                    if (Input.GetKeyDown(KeyCode.LeftShift) && conversationScript.state == conversationState.none) {
                        conversationScript.handleInteractionWith(facing.GetComponent<Npc>());
                    }
                }
                else if (hit.transform.CompareTag("Container")) {
                    //If player isn't already in a menu and presses shift
                    if (Input.GetKeyDown(KeyCode.LeftShift) && inventoryManager.state == inventoryState.none) {
                        inventoryManager.showContainerItems(facing.GetComponent<MurderMystery.ContainerScript>());
                        inventoryManager.justOpened = true;
                    }
                }
            }
            else {
                facing = null;
            }
        }

        void OnTriggerStay2D(Collider2D other) {
            if (other.transform.parent.name == "Doorways") {
                if (Input.GetKeyDown(KeyCode.LeftShift)) {
                    other.GetComponent<MurderMystery.DoorwayScript>().Travel();
                }

            }

            if (other.transform.parent.name == "Rooms") {
                currentRoom = other.gameObject;
            }

        }


    }

    

}