using UnityEngine;
using System.Collections.Generic;


public class ItemManager : MonoBehaviour {

    public List<Item> itemsPool;
    public List<GameObject> itemPrefabs;

    void Awake() {
        itemsPool = new List<Item>();
        itemsPool.Add(new Weapon("Kitchen Knife", Weapon.DamageType.stab));
    }

    public Item createItem(int i, Room room) {
        Item item = (Item)itemsPool[i];
        item.room = room;
        room.items.Add(item);
        return item;
    }
}

[System.Serializable]
public abstract class Item {

    public bool held;
    public string name;
    public Room room;

}

[System.Serializable]
public class Weapon : Item {

    public enum DamageType { blunt, stab, poison, shot }
    public DamageType damageType;

    public Weapon(string name, DamageType damageType) {
        this.name = name;
        this.damageType = damageType;
    }

}
