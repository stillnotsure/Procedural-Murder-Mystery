using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugRoomDisplay : MonoBehaviour {

    private bool running;
    PlotGenerator plotGenerator;
    public List<NPCLog> logs;
    int number_of_NPCs;

     void Start() {
        running = false;
        logs = new List<NPCLog>();
        plotGenerator = GetComponent<PlotGenerator>();
        number_of_NPCs = plotGenerator.number_of_characters;

        for (int i = 0; i < number_of_NPCs; i++) {
            int column = i % (number_of_NPCs / 2);

            int row = 0;
            if (i >= number_of_NPCs / 2) row = 1;
            Rect rect = new Rect(
                column * (Screen.width / (number_of_NPCs / 2) ),
                row * (Screen.height / 2),

                Screen.width / (number_of_NPCs / 2),
                Screen.height / 2);

            NPCLog log = new NPCLog(plotGenerator.npcs[i], rect);
            logs.Add(log);
            plotGenerator.npcs[i].log = log;
        }
    }

    void OnGUI() {
        foreach (NPCLog log in logs) {
            GUI.contentColor = log.color;
            GUI.Label( log.rect, log.Mytext, GUI.skin.textArea );
            GUI.backgroundColor = Color.white;
            GUI.Label(log.nameRect, log.npc.firstname + " " + log.npc.surname, GUI.skin.textArea);
        }
    }

    void begin() {
        running = true;
    }

}

public class NPCLog {

    [SerializeField]
    public Rect rect;
    public Rect nameRect;
    public Npc npc;
    public int maxLines = 9;
    private Queue<string> queue = new Queue<string>();
    public string Mytext = "";

    public Color color;

    public NPCLog(Npc npc, Rect rect) {
        this.npc = npc;
        nameRect = rect;
        nameRect.height = 20;

        rect.height -= 20;
        rect.y = rect.y + 20;

        this.rect = rect;
        if (npc.isMurderer) { color = Color.red; }
        else if (npc.isVictim) { color = Color.cyan; }
        else { color = Color.green; }
    }

    public void NewActivity(string activity) {
        if (queue.Count >= maxLines)
            queue.Dequeue();

        queue.Enqueue(activity);

        Mytext = "";
        foreach (string st in queue)
            Mytext = Mytext + st + "\n\n";
    }

}
