using UnityEngine;
using System.Collections;

public class ConversationScript : MonoBehaviour {

    public GameObject textbox;
    public bool speechHappening;
    public float letterDelay = 0.1f;

    private string fullString;
    private string shownString;

	// Use this for initialization
	void Start () {
        speechHappening = false;
        textbox = GameObject.Find("Textbox");
	}
	
	// Update is called once per frame
	void Update () {
	   
	}

    void OnGUI()
    {
        if (speechHappening)
        {
            GUI.Box(new Rect(10, 10, Screen.width / 2, Screen.height / 4), shownString);
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

    void CreateTextBox()
    {
       
    }

}
