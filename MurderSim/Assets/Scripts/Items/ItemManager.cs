using UnityEngine;
using System.Collections.Generic;


public class ItemManager : MonoBehaviour {

    public List<GameObject> itemsPool;  //List of all items in the gameworld
    public List<GameObject> itemPrefabs;    //List of all possible items, for spawning

    void Awake() {
        itemsPool = new List<GameObject>();
    }

    public GameObject createItem(int i, Room room) {
        GameObject item = Instantiate(itemPrefabs[i]);
        Item itemScript = item.GetComponent<Item>();

        itemScript.room = room;
        room.items.Add(item);
        return item;
    }
}

[System.Serializable]
public abstract class Item : MonoBehaviour {

    public bool held;
    public Room room;

}
