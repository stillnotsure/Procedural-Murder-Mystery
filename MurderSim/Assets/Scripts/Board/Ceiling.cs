using UnityEngine;
using System.Collections;

public class Ceiling : MonoBehaviour {

    public GameObject room;
    public string roomName;

    void Start() {
        room = GameObject.Find(roomName);
    }

    public void makeVisible() {
        gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
    }

    public void makeInvisible() {
        gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
    }

}
