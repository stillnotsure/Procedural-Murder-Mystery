using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public enum inventoryState { none, playerInventory, container, npcInventory };

public class InventoryManager : MonoBehaviour {

    public inventoryState state;
    private List<GameObject> items;
    private List<Image> images;

    //UI
    public int selected = 0;
    public GameObject inventoryPanel;
    public Text selectedItemText;


    void Start () {
        items = new List<GameObject>();
        images = new List<Image>();
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            setStateNone();
        }
        if (state != inventoryState.none) {
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
            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                selectItem(selected);
            }
        }
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

    }

    private void setStateNone() {
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
        images[selected].color = new Color(1f, 1f, 1f, 1f);
    }

    private void highlightSelected() {
        selectedItemText.text = items[selected].GetComponent<Item>().name;
        StartCoroutine(FadeOut(images[selected], 0.35f));
    }

    private void displayItems() {
        images.Clear();
        clearPanel();

        foreach (GameObject item in items) {
            GameObject go = new GameObject(item.GetComponent<Item>().name);
            Image image = go.AddComponent<Image>();
            image.sprite = item.GetComponent<SpriteRenderer>().sprite;
            go.transform.SetParent(inventoryPanel.transform, false);
            images.Add(image);
        }

        selected = 0;
        deSelect();
        highlightSelected();
    }

    public void showContainerItems(MurderMystery.ContainerScript container) {
        state = inventoryState.container;
        List<GameObject> containerItems = container.items;
        items.Clear();
        foreach (GameObject item in containerItems) {
            items.Add(item);
        }
        displayItems();
    }

    IEnumerator FadeIn(Image image, float aTime) {
        Debug.Log("Fading in");
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(image.color.a, 1.00f, t));
            image.color = newColor;
            yield return null;
        }
        yield return null;
        StartCoroutine(FadeOut(image, aTime));
    }

    IEnumerator FadeOut(Image image, float aTime) {
        Debug.Log("Fading out");

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(image.color.a, 0.00f, t));
            image.color = newColor;
            yield return null;
        }
        yield return null;
        StartCoroutine(FadeIn(image, aTime));
    }
}
