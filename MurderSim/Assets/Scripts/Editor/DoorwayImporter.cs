using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[Tiled2Unity.CustomTiledImporter]

class CustomImporterAddComponent : Tiled2Unity.ICustomTiledImporter {
    public void HandleCustomProperties(UnityEngine.GameObject gameObject,
        IDictionary<string, string> props) {
        
        if (props.ContainsKey("TargetDoor")) {
            gameObject.AddComponent<MurderMystery.DoorwayScript>();
            gameObject.GetComponent<MurderMystery.DoorwayScript>().targetDoor = props["TargetDoor"];
        }
        
        if (props.ContainsKey("Room")) {
            gameObject.AddComponent<MurderMystery.ContainerScript>();
            gameObject.GetComponent<MurderMystery.ContainerScript>().roomName = props["Room"];
            gameObject.tag = "Container";
            gameObject.layer = LayerMask.NameToLayer("Containers");
        }
        
    }

    public void CustomizePrefab(GameObject prefab) {
        // Do nothing
    }
}