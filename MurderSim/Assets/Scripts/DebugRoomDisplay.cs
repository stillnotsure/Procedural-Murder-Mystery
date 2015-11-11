using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugRoomDisplay : MonoBehaviour {

    PlotGenerator plotGenerator;
    int number_of_NPCs;

    public void run() {
        plotGenerator = GetComponent<PlotGenerator>();
        number_of_NPCs = plotGenerator.number_of_characters;

        for (int i = 0; i < number_of_NPCs; i++) {
            
            int row = 0;
            int column = i % number_of_NPCs / 2;

            if (i > number_of_NPCs / 2) row = 1;
            Rect rect = new Rect(column * (Screen.width / (number_of_NPCs / 2)), row * Screen.height / 2, Screen.width / (number_of_NPCs / 2), row * Screen.height / 2);
        }
    }

}

public class NPCLog : MonoBehaviour {

    public Rect rect;
    public string NPCName;
    public int maxLines = 8;
    private Queue<string> queue = new Queue<string>();
    private string Mytext = "";

    public NPCLog(string NPCName, Rect rect) {
        this.NPCName = NPCName;
        this.rect = rect;
    }

    public void NewActivity(string activity) {
        if (queue.Count >= maxLines)
            queue.Dequeue();

        queue.Enqueue(activity);

        Mytext = "";
        foreach (string st in queue)
            Mytext = Mytext + st + "\n";
    }


    void OnGUI() {
        GUI.Label(new Rect(rect.x,                             // x, left offset
                    (rect.y),            // y, bottom offset
                     rect.width,                                // width
                     rect.height), Mytext, GUI.skin.textArea);    // height, text, Skin features
    }
}
