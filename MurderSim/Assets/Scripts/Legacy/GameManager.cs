using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public BoardManager boardScript;
    public ConversationScript conversationScript;

	// Use this for initialization
	void Start () {
        boardScript = GetComponent<BoardManager>();
        conversationScript = GetComponent<ConversationScript>();
        InitGame();
	}
	
    void InitGame()
    {
        //boardScript.SetupScene();
    }

	// Update is called once per frame
	void Update () {
        Debug();
	}

    void Debug()
    {
        if (Input.GetKeyDown("`"))
        {
            conversationScript.displayDebugText();
        }
    }
}
