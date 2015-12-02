using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = player.transform.position;
        newPos.z = -10;
        this.transform.position = newPos;
	}
}
