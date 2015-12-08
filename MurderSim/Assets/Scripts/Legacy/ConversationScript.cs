using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public enum dialog {greeting, alibi};
public enum conversationState { none, moreText, playerInput, npcSpeaking};

public class ConversationScript : MonoBehaviour {

    public conversationState state;
    public float letterDelay = 0.08f;

    //References
    private Npc speakingNPC;
    public GameObject textPanel;
    public Text TextArea;
    public Text responseArea;
    public Text nameText;

    //Responses
    private List<String> responses;
    public int selected;

    //Text holders
    private Queue<string> dialogueQueue;
    private string fullString;
    private string shownString;

    void Start() {
        state = conversationState.none;
        responses = new List<string>();
        dialogueQueue = new Queue<string>();
        setUpDialogOptions();
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
                state = conversationState.playerInput;
            }
            StopAllCoroutines();
        }

    }

    void setUpDialogOptions() {
        responses.Add("Where were you at the time of the murder?");
        responses.Add("What did you see around the time of the murder?");
        responses.Add("Is there anyone you suspect?");
        responses.Add("Cancel");
    }

    void NPCAlibi() {
        List<Event> events = Timeline.fullNPCHistory(speakingNPC.GetComponent<Npc>());
        if (events.Count == 0)
            displayText("I've been here the whole evening.");

        else {
            foreach (Event e in Timeline.fullNPCHistory(speakingNPC.GetComponent<Npc>())) {
                dialogueQueue.Enqueue(e.toString());
            }
            displayText(dialogueQueue.Dequeue());
        }
    }

    void NPCGreeting(Npc npc) {
        speakingNPC = npc;
        displayText("Hi.");
    }

    public void startConversationWith(Npc npc) {
        NPCGreeting(npc);
    }
}
