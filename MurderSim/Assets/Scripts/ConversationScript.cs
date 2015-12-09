using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public enum dialog {greeting, alibi};
public enum conversationState { none, moreText, playerInput, npcSpeaking};

public class ConversationScript : MonoBehaviour {

    public conversationState state;
    public float letterDelay = 0.04f;

    //References
    public Npc speakingNPC;
    public GameObject textPanel;
    public Text TextArea;
    public Text responseArea;
    public Text nameText;
    private PlotGenerator pg;

    //Responses
    private List<String> responses;
    public int selected;

    //Text holders
    private Queue<string> dialogueQueue;
    private string fullString;
    private string shownString;

    void Start() {
        pg = gameObject.GetComponent<PlotGenerator>();

        state = conversationState.none;
        responses = new List<string>();
        dialogueQueue = new Queue<string>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            setStateNone();
        }
        //More text is set when the current line is finished printing, but there is more dialogue from the NPC waiting to be displayed upon pressing shift
        if (state == conversationState.moreText) {
            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                displayText(dialogueQueue.Dequeue());
            }
        } 

        //Player input is when the NPC has finished talking and the player uses the menu to respond to them
        else if (state == conversationState.playerInput) {
            if (Input.GetKeyDown("w")) {
                selected = Math.Max(0, selected - 1);
            }
            else if (Input.GetKeyDown("s")) {
                selected = Math.Min(responses.Count - 1, selected + 1);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                selectResponse(selected);
            }
        }
    }

    void OnGUI() {

        if (state != conversationState.none) {
            textPanel.SetActive(true);
            nameText.text = speakingNPC.firstname + " " + speakingNPC.surname;
        }

        if (state == conversationState.npcSpeaking) {
            responseArea.text = "";
            TextArea.text = shownString;
        }
        else if (state == conversationState.playerInput) {

            string responseText = "";
            for (int i = 0; i < responses.Count; i++) {
                if (selected == i)
                    responseText += String.Format("<b> {0}. {1} </b>\n", i + 1, responses[i]);
                else
                    responseText += String.Format("{0}. {1} \n", i + 1, responses[i]);
            }
            responseArea.text = responseText;
        }
        else if (state == conversationState.moreText) {
            string responseText = "Press Shift to Continue";
            responseArea.text = responseText;
        }
        else if (state == conversationState.none) {
            nameText.text = "";
            responseArea.text = "";
            TextArea.text = "";
            textPanel.SetActive(false);
        }

    }

    void setStateNone() {
        state = conversationState.none;
        selected = 0;
        shownString = "";
        fullString = "";
        dialogueQueue.Clear();
    }

    void selectResponse(int i) {
        selected = 0;
        string selectedText = responses[i];

        if (selectedText.Equals("Where were you at the time of the murder?")) {
            NPCAlibi();
        }
        else if (selectedText.Equals("What did you see around the time of the murder?")) {
            Debug.Log("Selected witness");
            NPCWitnessed();
        }
        else if (selectedText.Equals("Cancel")) {
            setStateNone();
        }
    }

    public void displayText(string text) {
        StopAllCoroutines();
        shownString = "";
        fullString = text;

        state = conversationState.npcSpeaking;
        StartCoroutine(RevealString());
    }

    IEnumerator RevealString() {

        foreach (char letter in fullString.ToCharArray()) {
            shownString += letter;
            yield return new WaitForSeconds(letterDelay);
        }

        if (shownString == fullString) {
            if (dialogueQueue.Count > 0) {
                state = conversationState.moreText;
            } else {
                setUpDialogOptions();
                state = conversationState.playerInput;
            }
            StopAllCoroutines();
        }

    }

    void setUpDialogOptions() {
        responses.Clear();
        if (speakingNPC.isAlive) {
            responses.Add("Where were you at the time of the murder?");
            responses.Add("What did you see around the time of the murder?");
            responses.Add("Is there anyone you suspect?");
        } 
        else {
            Debug.Log("npc is dead");
            responses.Add("Examine the injuries");
            responses.Add("Estimate the time of death");
        }
        
        responses.Add("Cancel");
    }

    void NPCAlibi() {
        List<Event> events = Timeline.locationDuringTimeframe(speakingNPC, Timeline.murderEvent.time - (pg.timeOfDeathLeeway / Timeline.timeIncrements), Timeline.murderEvent.time + (pg.timeOfDeathLeeway / Timeline.timeIncrements));
        if (events.Count == 0)
            displayText("I've been here the whole evening.");

        else {
            foreach (SwitchRooms e in events) {
                string s = String.Format("At {0} I moved to the {1}", Timeline.convertTime(e.time), e.newRoom.roomName);
                dialogueQueue.Enqueue(s);
            }
            displayText(dialogueQueue.Dequeue());
        }
    }

    void NPCWitnessed() {
        Debug.Log("Witness running");
        List<Event> events = Timeline.EventsWitnessedDuringTimeframe(speakingNPC, Timeline.murderEvent.time - (pg.timeOfDeathLeeway / Timeline.timeIncrements), Timeline.murderEvent.time + (pg.timeOfDeathLeeway / Timeline.timeIncrements));
        if (events.Count == 0)
            displayText("I haven't seen anything unusual");
        else {
            for (int i = 0; i < events.Count; i++) {
                if (events[i] is SwitchRooms) {
                    SwitchRooms e = events[i] as SwitchRooms;
                    dialogueQueue.Enqueue(String.Format("At {0} I saw {1} move into the {2}", Timeline.convertTime(events[i].time), e.npc.getFullName(), e.newRoom.roomName));
                }
                else if(events[i] is FoundBody) {
                    FoundBody e = events[i] as FoundBody;
                    dialogueQueue.Enqueue("Witnessed someone finding the dead body?!");
                }
                else if (events[i] is Encounter) {
                    Encounter e = events[i] as Encounter;
                    dialogueQueue.Enqueue(String.Format("At {0} I saw {1} meet {2} in the {3}", Timeline.convertTime(events[i].time), e.npc.getFullName(), e.npc2.getFullName(), e.room.roomName));
                }
                else if (events[i] is Murder) {
                    Murder e = events[i] as Murder;
                    dialogueQueue.Enqueue(String.Format("At {0} I saw {1} murder {2} in the {3}! I swear it's true!", Timeline.convertTime(events[i].time), e.npc.getFullName(), e.npc2.getFullName(), e.room.roomName));
                }
                else if (events[i] is PickupItem) {
                    PickupItem e = events[i] as PickupItem;
                    dialogueQueue.Enqueue(String.Format("At {0} I saw {1} pick up a {2} in the {3}", Timeline.convertTime(events[i].time), e.npc.getFullName(), e.item.name, e.room.roomName));
                }
                else if (events[i] is DropItem) {
                    DropItem e = events[i] as DropItem;
                    dialogueQueue.Enqueue(String.Format("At {0} I saw {1} drop a {2} in the {3}", Timeline.convertTime(events[i].time), e.npc.getFullName(), e.item.name, e.room.roomName));
                }
            }
            displayText(dialogueQueue.Dequeue());
        }
        
    }

    void NPCGreeting(Npc npc) {
        displayText("Hi.");
    }

    void examineBody(Npc npc) {
        displayText(npc.getFullName() + "'s lifeless body lies before you");
    }

    public void handleInteractionWith(Npc npc) {
        speakingNPC = npc;
        if (npc.isAlive)
            NPCGreeting(npc);
        else
            examineBody(npc);
    }
}
