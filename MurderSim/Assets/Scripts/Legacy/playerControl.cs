using UnityEngine;
using System.Collections;

public class playerControl : MonoBehaviour {

    public float playerSpeed = 2.0f;
    public BoxCollider2D box;
    private string lastDirection;

    //private ConversationScript conversationScript;

    
    [SerializeField] private GameObject facingNPC;

	// Use this for initialization
	void Start () {
        // conversationScript = GetComponent<ConversationScript>();
        box = GetComponent<BoxCollider2D>();
        lastDirection = "up";
        facingNPC = null;
	}

    void FixedUpdate() {
        Move();
    }

    void Move() {
        float horiz = Input.GetAxisRaw("Horizontal");
        float vertic = Input.GetAxisRaw("Vertical");

        transform.Translate(horiz * 5 * Time.deltaTime, vertic * 5 * Time.deltaTime, 0);
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.transform.parent.name == "Doorways") {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                other.GetComponent<MurderMystery.DoorwayScript>().Travel();
        }

        if (other.transform.parent.name == "Rooms") {
            //Debug.Log(other.name);
        }
    }

}


