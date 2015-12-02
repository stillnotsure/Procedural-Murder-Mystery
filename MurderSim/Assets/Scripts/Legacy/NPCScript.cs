using UnityEngine;
using System.Collections;

public class NPCScript : MonoBehaviour {

	private string name;

	//Mental Character attributes
	[SerializeField] private int trustworthiness, friendliness, courageousness, dangerousness;

	//Physical Character attributes
	private string gender;

	// Use this for initialization
	void Start () {
        generateAttributes();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void generateAttributes(){
        trustworthiness = Random.Range(1, 101);
        friendliness = Random.Range(1, 101);
        courageousness = Random.Range(1, 101);
        dangerousness = Random.Range(1, 101);
    }

}
