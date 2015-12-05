using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {


    public float speed = 8.0f;
    private Vector3 pos;
    private Transform tr;

    void Start() {
        //Here we set the Values from the current position
        pos = transform.position;
        tr = transform;
    }

    void Update() {
        Movement();
    }



    private void Movement() {
        bool spaceIsFree = false;
        if (Input.GetKey("w")) { spaceIsFree = checkCollisions("up"); }
        if (Input.GetKey("s")) { spaceIsFree = checkCollisions("down"); }
        if (Input.GetKey("a")) { spaceIsFree = checkCollisions("left"); }
        if (Input.GetKey("d")) { spaceIsFree = checkCollisions("right"); }

        if (spaceIsFree) {
            //But we Check if we are at the new Position, before we can add some more
            //it will prevent to move before you are at your next 'tile'
            if (Input.GetKey(KeyCode.D) && tr.position == pos && checkCollisions("right") ) {
                pos += Vector3.right;
            }
            else if (Input.GetKey(KeyCode.A) && tr.position == pos && checkCollisions("left")) {
                pos += Vector3.left;
            }
            else if (Input.GetKey(KeyCode.W) && tr.position == pos && checkCollisions("up")) {
                pos += Vector3.up;
            }
            else if (Input.GetKey(KeyCode.S) && tr.position == pos && checkCollisions("down")) {
                pos += Vector3.down;
            }

        }

        //Here you will move Towards the new position ...
        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);


    }


    bool checkCollisions(string direction) {
        Vector2 origin = transform.position;
        Vector2 rayDirection = origin;

        switch (direction) {
            case "up":
                rayDirection = Vector2.up;
                break;
            case "right":
                rayDirection = Vector2.right;
                break;
            case "down":
                rayDirection = Vector2.down;
                break;
            case "left":
                rayDirection = Vector2.left;
                break;
        }

        Ray ray = new Ray(origin, rayDirection);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 2.0f);
        Debug.DrawRay(ray.origin, ray.direction, Color.green, 1.0f, true); //Maybe set to false later

        if (hit && hit.collider.gameObject != this.gameObject) {
            Debug.Log("hit something");
            return false;
        }
        else {
            return true;
        }
    }

}