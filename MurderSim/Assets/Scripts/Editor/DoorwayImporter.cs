using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[Tiled2Unity.CustomTiledImporter]

class CustomImporterAddComponent : Tiled2Unity.ICustomTiledImporter {
    public void HandleCustomProperties(UnityEngine.GameObject gameObject,
        IDictionary<string, string> props) {
        // Simply add a component to our GameObject
        if (props.ContainsKey("TargetDoor")) {
            gameObject.AddComponent<MurderMystery.DoorwayScript>();
            gameObject.GetComponent<MurderMystery.DoorwayScript>().targetDoor = props["TargetDoor"];
        }
    }

    public void CustomizePrefab(GameObject prefab) {
        // Do nothing
    }
}