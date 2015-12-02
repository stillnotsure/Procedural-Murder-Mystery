using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum dialog {greeting, alibi};

public class ConversationScript : MonoBehaviour {

    public GameObject textbox;
    public bool speechHappening;
    public float letterDelay = 0.1f;

    private string fullString;
    private string shownString;

    
    private Dictionary<dialog, List<string>> dialogs;

	// Use this for initialization
	void Start () {
        fullString = "";
        shownString = "";
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
        if (speechHappening)
        {
            GUI.Box(new Rect(10, 10, Screen.width / 2, Screen.height / 4), shownString);

            if (shownString.Equals(fullString))
            {
                GUI.Box(new Rect(10, 15 + Screen.height / 4, Screen.width / 2, 40), "Debug");
            }

        } 
        
       // GUI.Label(new Rect (10,60,400,50), shownString);
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
