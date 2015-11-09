using UnityEngine;
using System.Collections.Generic;

public class Event {

    public enum Types {switchRooms, pickup, putdown, murder};
    public Types type;
    public Time time;

    public Event(Types type, Time time) {
        this.type = type;
        this.time = time;
    }
}

public class Timeline : MonoBehaviour {

    public List<Event> events;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void addEvent(Event e){
        events.Add(e);
    }
}
