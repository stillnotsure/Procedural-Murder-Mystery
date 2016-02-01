using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MurderMystery {

    public class UIManager : MonoBehaviour {

        PlotGenerator pg;

        //Relations Panel
        GameObject debugPanel;
        GameObject[] relationPanels;

        // Use this for initialization
        void Start() {
            pg = GameObject.Find("GameManager").GetComponent<PlotGenerator>();
            relationPanels = new GameObject[pg.number_of_characters];
            debugPanel = GameObject.Find("DebugPanel");

            displayRelationManager(false);
        }

        // Update is called once per frame
        void Update() {

        }

        public void setupRelationPanel() {
            for (int i = 0; i < pg.npcs.Count; i++) {
                GameObject relationPanel = (GameObject)Instantiate(Resources.Load("UI/RelationPanel"));
                relationPanel.transform.SetParent(debugPanel.transform);
                relationPanels[i] = relationPanel;
                relationPanel.transform.FindChild("RelationName").GetComponent<Text>().text = pg.npcs[i].getFullName();
                relationPanel.GetComponent<RelationSliderScript>().npcIndex = i;
            }
        }

        public void setRelationships(Npc npc) {
            
            for (int i = 0; i < pg.npcs.Count; i++) {
                relationPanels[i].GetComponent<RelationSliderScript>().changedByUIManager = true;

                int relationLevel = pg.relationships[pg.npcs.IndexOf(npc), i];
                GameObject relationPanel = relationPanels[i];
                relationPanel.transform.FindChild("RelationSlider").GetComponent<Slider>().value = relationLevel;
                relationPanel.transform.FindChild("RelationValue").GetComponent<Text>().text = relationLevel.ToString();
            }
        }

        public void displayRelationManager(bool visible) {
            if (debugPanel != null) debugPanel.SetActive(visible);
        }

    }
}