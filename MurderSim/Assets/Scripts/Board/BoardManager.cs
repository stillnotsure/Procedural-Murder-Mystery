using UnityEngine;
using System.Collections;
using SpriteTile;

public class BoardManager : MonoBehaviour {

    public Camera tileCam;
    public TextAsset level;

    void Awake () {
        Tile.SetCamera(tileCam);
        Tile.LoadLevel(level);
        Tile.SetLayerPosition(1, new Vector2(-10.0f, -3.7f));
        Tile.SetLayerPosition(0, new Vector2(10.0f, 6.6f));
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
