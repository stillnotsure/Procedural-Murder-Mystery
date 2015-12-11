using UnityEngine;
using System.Collections.Generic;


public class ItemManager : MonoBehaviour {

    private Mansion mansion;
    public List<GameObject> itemsPool;  //List of all items in the gameworld
    public List<GameObject> itemPrefabs;    //List of all possible items, for spawning

    void Awake() {
        itemsPool = new List<GameObject>();
        mansion = gameObject.GetComponent<Mansion>();
    }

    public GameObject createItem(int i, Room room) {
        GameObject item = Instantiate(itemPrefabs[i]);
        Item itemScript = item.GetComponent<Item>();

        itemScript.room = room;
        room.items.Add(item);
        return item;
    }

    public void placeItemsOnBoard() {

        foreach (Room room in mansion.rooms) {

            foreach(GameObject item in room.items) {
                //Find the room the item should be placed in
                Room targetRoom = item.GetComponent<Item>().room;
                List<GameObject> containers = targetRoom.containers;

                //If item is allocated to a container, place it in it

                //Otherwise, find all the containers in the room and allocate one randomly

                /*
                if (targetRoom.GetComponent<BoxCollider2D>() != null) {
                    BoxCollider2D collider = targetRoom.GetComponent<BoxCollider2D>();
                    Bounds bounds = collider.bounds;
                    float x = Random.Range(bounds.min.x, bounds.max.x);
                    float y = Random.Range(bounds.min.y, bounds.max.y);

                    item.transform.Translate(new Vector3(x, y, 0));
                }
                */

            }

        }

    }

}

[System.Serializable]
public abstract class Item : MonoBehaviour {

    public enum ItemState {held, dropped, contained, onDisplay}

    public ItemState state;
    public Room room;

}
