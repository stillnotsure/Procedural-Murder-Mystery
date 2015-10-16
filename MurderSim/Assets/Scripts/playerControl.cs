using UnityEngine;
using System.Collections;

public class playerControl : MonoBehaviour {

    public float playerSpeed = 2.0f;
    public BoxCollider2D box;
    private string lastDirection;

    private ConversationScript conversationScript;

    
    [SerializeField] private bool facingNPC;

	// Use this for initialization
	void Start () {
        conversationScript = GetComponent<ConversationScript>();

        box = GetComponent<BoxCollider2D>();
        lastDirection = "up";
        facingNPC = false;
	}
	
	// Update is called once per frame
	void Update () {
        checkCollisions();
        getInput();
        Move();
    }

    void getInput()
    {
        if (facingNPC)
        {
            if (Input.GetKeyDown("space"))
            {
                conversationScript.displayDebugText();
            }
        }
    }

    void Move()
    {
        float horiz = Input.GetAxisRaw("Horizontal");
        float vertic = Input.GetAxisRaw("Vertical");

        transform.Translate(horiz * playerSpeed * Time.deltaTime, vertic * playerSpeed * Time.deltaTime, 0);

        if (Input.GetKey("w")) { lastDirection = "up"; }
        if (Input.GetKey("s")) { lastDirection = "down"; }
        if (Input.GetKey("a")) { lastDirection = "left"; }
        if (Input.GetKey("d")) { lastDirection = "right"; }
    }

    void checkCollisions()
    {
        Vector2 origin = transform.position;
        Vector2 direction = origin;
        switch (lastDirection)
        {
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
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1.0f);
        Debug.DrawRay(ray.origin, ray.direction, Color.green, 1.0f, true); //Maybe set to false later

        if (hit)
        {
            if (hit.transform.name.Equals("Npc"))
            {
                Debug.Log("Hit NPC");
                facingNPC = true;
            }
        }
        else {
            facingNPC = false;
            conversationScript.speechHappening = false;
        }
    }
}


