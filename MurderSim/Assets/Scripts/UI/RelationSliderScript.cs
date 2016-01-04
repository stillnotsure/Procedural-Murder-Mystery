using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MurderMystery {

    public class RelationSliderScript : MonoBehaviour {

        private PlotGenerator pg;
        public int npcIndex;

        public bool changedByUIManager;

        void Start() {
            changedByUIManager = false;
            pg = GameObject.Find("GameManager").GetComponent<PlotGenerator>();
        }

        public void changedSlider(Slider slider) {
            if (!changedByUIManager) {
                gameObject.transform.FindChild("RelationValue").GetComponent<Text>().text = slider.value.ToString();
                Npc targetNpc = pg.gameObject.GetComponent<ConversationScript>().speakingNPC;

                Debug.Log("Target NPC: " + pg.npcs.IndexOf(targetNpc) + " Slider NPC : " + npcIndex);
                pg.relationships[pg.npcs.IndexOf(targetNpc), npcIndex] = (int)slider.value;
            }
            else {
                changedByUIManager = false;
            }
        }
    }

}
