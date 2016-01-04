using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace MurderMystery {
    public enum inventoryState { none, playerInventory, container, npcInventory };

    public class InventoryManager : MonoBehaviour {

        public inventoryState state;
        private List<GameObject> items;
        private List<Image> images;

        //UI
        public int selected = 0;
        public GameObject inventoryPanel;
        public Text selectedItemText;

        //Player Specific
        public bool justOpened = false;
        public GameObject facing;
        public List<GameObject> playerInventory;


        void Start() {
            //references
            selectedItemText = GameObject.Find("SelectedItemText").GetComponent<Text>();
            inventoryPanel = GameObject.Find("ContainerPanel");

            items = new List<GameObject>();
            images = new List<Image>();
            playerInventory = new List<GameObject>();
        }

        void Update() {

            if (Input.GetKeyDown(KeyCode.LeftControl)) {
                setStateNone();
            }
            if (state != inventoryState.none && items.Count > 0) {
                if (Input.GetKeyDown("a")) {
                    deSelect();
                    selected = Math.Max(0, selected - 1);
                    highlightSelected();
                }
                else if (Input.GetKeyDown("d")) {
                    deSelect();
                    selected = Math.Min(images.Count - 1, selected + 1);
                    highlightSelected();
                }
                if (Input.GetKeyDown(KeyCode.LeftShift) && !justOpened) {
                    selectItem(selected);
                }
            }
            else {
                if (Input.GetKeyDown("i")) {
                    OpenInventory();
                }
            }

            if (justOpened) justOpened = false;
        }

        void OnGUI() {
            if (state != inventoryState.none) {
                selectedItemText.gameObject.SetActive(true);
                inventoryPanel.SetActive(true);
            }
            else {
                selectedItemText.gameObject.SetActive(false);
                inventoryPanel.SetActive(false);
            }
        }

        private void selectItem(int selected) {
            if (state == inventoryState.container || state == inventoryState.npcInventory) {

                if (selected > images.Count - 1) selected = images.Count - 1;

                playerInventory.Add(items[selected]);
                facing.GetComponent<ContainerScript>().items.Remove(items[selected]);
                StopAllCoroutines();
                showContainerItems(facing.GetComponent<ContainerScript>());

            }
        }

        private void setStateNone() {
            facing = null;
            state = inventoryState.none;
            deSelect();
            selected = 0;
            images.Clear();
            items.Clear();
            clearPanel();
        }

        private void clearPanel() {
            foreach (Transform child in inventoryPanel.transform) {
                GameObject.Destroy(child.gameObject);
            }
        }

        private void deSelect() {
            StopAllCoroutines();
            selectedItemText.text = "";
            if (images.Count > 0)
                images[selected].color = new Color(1f, 1f, 1f, 1f);
        }

        private void highlightSelected() {
            selectedItemText.text = items[selected].GetComponent<Item>().name;
            StartCoroutine(FadeOut(images[selected], 0.35f));
        }

        private void displayItems() {
            images.Clear();
            clearPanel();

            if (items.Count > 0) {
                foreach (GameObject item in items) {
                    GameObject go = new GameObject(item.GetComponent<Item>().name);
                    Image image = go.AddComponent<Image>();
                    image.preserveAspect = true;
                    image.sprite = item.GetComponent<SpriteRenderer>().sprite;
                    go.transform.SetParent(inventoryPanel.transform, false);
                    images.Add(image);
                }

                selected = 0;
                deSelect();
                highlightSelected();
            }

            else {
                selectedItemText.text = "Empty";
            }

        }

        private void OpenInventory() {
            state = inventoryState.playerInventory;
            items.Clear();
            foreach (GameObject item in playerInventory) {
                items.Add(item);
            }
            displayItems();
        }

        public void showContainerItems(MurderMystery.ContainerScript container) {
            Debug.Log("Looking into " + container.name);
            facing = container.gameObject;
            state = inventoryState.container;
            List<GameObject> containerItems = container.items;
            items.Clear();
            foreach (GameObject item in containerItems) {
                Debug.Log("Adding " + item.name + " to container inventory panel");
                items.Add(item);
            }
            displayItems();
        }

        IEnumerator FadeIn(Image image, float aTime) {
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(image.color.a, 1.00f, t));
                image.color = newColor;
                yield return null;
            }
            yield return null;
            StartCoroutine(FadeOut(image, aTime));
        }

        IEnumerator FadeOut(Image image, float aTime) {
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(image.color.a, 0.00f, t));
                image.color = newColor;
                yield return null;
            }
            yield return null;
            StartCoroutine(FadeIn(image, aTime));
        }
    }
}