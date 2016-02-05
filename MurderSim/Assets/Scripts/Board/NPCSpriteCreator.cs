using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MurderMystery {

    public class NPCSpriteCreator : MonoBehaviour{

        public List<Sprite> maleBodies;
        public List<Sprite> femaleBodies;

        /*
        Old Random Face Generator, not used

        public List<Sprite> eyesList;
        public List<Sprite> mouthsList;
        public List<Sprite> hairList;
        */

        public void Start() {
            
        }

        public void getBody(Npc npc, GameObject go) {
            if (npc.gender == Npc.Gender.Male) {
                int i = Random.Range(0, maleBodies.Count);
                go.GetComponent<SpriteRenderer>().sprite = maleBodies[i];
                maleBodies.RemoveAt(i);
            } else {
                int i = Random.Range(0, femaleBodies.Count);
                go.GetComponent<SpriteRenderer>().sprite = femaleBodies[i];
                femaleBodies.RemoveAt(i);
            }
        }

        /*
        public void generateFace(Npc npc, GameObject go) {
            GameObject face = go.transform.FindChild("NPC Face").gameObject;
            GameObject head = face.transform.FindChild("Head").gameObject;
            GameObject hair = face.transform.FindChild("Hair").gameObject;
            GameObject mouth = face.transform.FindChild("Mouth").gameObject;
            GameObject eyes = face.transform.FindChild("Eyes").gameObject;

            int a = Random.Range(0, eyesList.Count);
            eyes.GetComponent<SpriteRenderer>().sprite = eyesList[a];
            int b = Random.Range(0, mouthsList.Count);
            mouth.GetComponent<SpriteRenderer>().sprite = mouthsList[b];
            int c = Random.Range(0, hairList.Count);
            hair.GetComponent<SpriteRenderer>().sprite = hairList[c];
        }
        */


    }
}
