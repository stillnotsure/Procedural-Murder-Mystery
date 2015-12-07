using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum dialog {greeting, alibi};

public class ConversationScript : MonoBehaviour {

    public GUISkin guiSkin;
    public GameObject textbox;
    public bool speechHappening;
    public float letterDelay = 0.1f;
    private GameObject activeNpc;

    private string fullString;
    private string shownString;
    private List<string> buttons;
    
    private Dictionary<dialog, List<string>> dialogs;

	// Use this for initialization
	void Start () {
        fullString = "";
        shownString = "";
        buttons = new List<string>();
        speechHappening = false;
        textbox = GameObject.Find("Textbox");
        dialogs = new Dictionary<dialog, List<string>>();
        setupAllDialogs();
	}
	
	// Update is called once per frame
	void Update () {
	   
	}

    void OnGUI()
    {
        GUI.skin = guiSkin;
        if (speechHappening)
        {
            Rect dialogue = new Rect(10, 10, Screen.width / 2, 50);
            GUI.Box(dialogue, shownString);

            if (shownString.Equals(fullString))
            {
                int yOffsetAmount = 5;
                int buttonHeight = 20;

                for (int i = 0; i < buttons.Count; i++) {
                    Rect button = new Rect(dialogue.x, dialogue.yMax + ( i * buttonHeight) + yOffsetAmount, dialogue.width, buttonHeight);
                    if (GUI.Button(button, buttons[i])) {
                        //Where were they at time of murder
                        if (i == 0) {
                            List<Event> history = Timeline.fullNPCHistory(activeNpc.GetComponent<Npc>());
                            displayText(history[0].toString());
                        }
                        if (i == 1) {
                            
                        }
                        if (i == 2) {

                        }
                    }
                }
            }
        } 
    }

    void setUpButtons() {
        buttons.Clear();
        buttons.Add("Where were you at the time of the murder?");
        buttons.Add("What did you see around the time of the murder?");
        buttons.Add("Is there anyone you suspect?");
    }

    IEnumerator RevealString()
    {
        foreach (char letter in fullString.ToCharArray())
        {
            shownString += letter;
            yield return new WaitForSeconds(letterDelay);
        }
    }

    public void displayDebugText()
    {
        StopAllCoroutines();
        shownString = "";

        fullString = "Testing, testing, 1 2 3.";
        StartCoroutine(RevealString());

        speechHappening = true;

    }

    public void displayText(string text)
    {
        StopAllCoroutines();
        shownString = "";

        fullString = text;
        StartCoroutine(RevealString());

        speechHappening = true;

    }

    public void startConversationWith(GameObject NPC)
	{
        activeNpc = NPC;
        List<string> outputs;
        string text = "";
        if (dialogs.TryGetValue(dialog.greeting, out outputs)) // Returns true.
        {
            int i = outputs.Count;
            if (i > 1)
            {
                int r = Random.Range(0, i);
                text = outputs[r];
            }
            else text = outputs[0];
        }

        setUpButtons();
        displayText(text);
    }

    void CreateTextBox()
    {
       
    }

    void setupAllDialogs()
    {
        //greeting dialogs
        var greetings = new List<string>();
        greetings.Add("Well hello there!");
        greetings.Add("How can I help you?");
        greetings.Add("What do you want?");

        //alibi dialogs
        var alibis = new List<string>();
        alibis.Add("When the body was found I was in the <alibi-room>.");
        alibis.Add("I was in the <alibi-room> when I heard the dreadful news.");

        dialogs.Add(dialog.greeting, greetings);
        dialogs.Add(dialog.alibi, alibis );
    }

}
