using UnityEngine;
using System.Collections;

public class playerControl : MonoBehaviour {

    //References
    public ConversationScript conversationScript;

    //Movement Vars
    public float playerSpeed = 2.0f;
    public BoxCollider2D box;
    private string lastDirection;

    private GameObject facingNPC;

	// Use this for initialization
	void Start () {
        box = GetComponent<BoxCollider2D>();
        lastDirection = "up";
        facingNPC = null;
	}

    void FixedUpdate() {
        checkCollisions();
        if (conversationScript.state == conversationState.none)
            Move();
    }

    void Move() {
        float horiz = Input.GetAxisRaw("Horizontal");
        float vertic = Input.GetAxisRaw("Vertical");

        transform.Translate(horiz * 5 * Time.deltaTime, vertic * 5 * Time.deltaTime, 0);

        if (Input.GetKey("w")) { lastDirection = "up"; }
        if (Input.GetKey("s")) { lastDirection = "down"; }
        if (Input.GetKey("a")) { lastDirection = "left"; }
        if (Input.GetKey("d")) { lastDirection = "right"; }
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
            if (hit.transform.CompareTag("NPC")) {
                facingNPC = hit.transform.gameObject;

                //If player isn't already in a conversation and presses shift
                if (Input.GetKeyDown(KeyCode.LeftShift) && conversationScript.state == conversationState.none) {

                    if (facingNPC.GetComponent<Npc>().isAlive)
                        conversationScript.startConversationWith(facingNPC.GetComponent<Npc>());
                    else
                        //TODO - Examine corpse - Could work with the conversation script, responses are different examinations, e.g. time of death, type of wound
                        Debug.Log("Examining the corpse");
                }
            }
        }
        else {
            facingNPC = null;
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.transform.parent.name == "Doorways") {
            if (Input.GetKeyDown(KeyCode.LeftShift) ) {
                other.GetComponent<MurderMystery.DoorwayScript>().Travel();
            }
                
        }

        if (other.transform.parent.name == "Rooms") {
            //Debug.Log(other.name);
        }

    }

}


