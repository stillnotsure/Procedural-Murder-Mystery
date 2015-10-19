using UnityEngine;
using System.Collections;

public class playerControl : MonoBehaviour {

    public float playerSpeed = 2.0f;
    public BoxCollider2D box;
    private string lastDirection;

    private ConversationScript conversationScript;

    
    [SerializeField] private GameObject facingNPC;

	// Use this for initialization
	void Start () {
        conversationScript = GetComponent<ConversationScript>();

        box = GetComponent<BoxCollider2D>();
        lastDirection = "up";
        facingNPC = null;
	}
	
	// Update is called once per frame
	void Update () {
        checkCollisions();
        getInput();

        if (!conversationScript.speechHappening)
            Move();
    }

    void getInput()
    {
        if (facingNPC != null)
        {
            if (Input.GetKeyDown("space"))
            {
				conversationScript.startConversationWith(facingNPC);
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
                facingNPC = hit.transform.gameObject;
            }
        }
        else {
            facingNPC = null;
            conversationScript.speechHappening = false;
        }
    }
}


