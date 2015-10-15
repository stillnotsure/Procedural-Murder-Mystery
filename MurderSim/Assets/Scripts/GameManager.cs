using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public BoardManager boardScript;

	// Use this for initialization
	void Start () {
        boardScript = GetComponent<BoardManager>();
        InitGame();
	}
	
    void InitGame()
    {
        boardScript.SetupScene();
    }
	// Update is called once per frame
	void Update () {
	
	}
}
