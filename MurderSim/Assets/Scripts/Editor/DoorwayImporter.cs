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
        
        if (props.ContainsKey("Ceiling")) {
            gameObject.layer = LayerMask.NameToLayer("Ceilings");
            gameObject.tag = "Ceiling";
            Ceiling ceiling = gameObject.AddComponent<Ceiling>();
            ceiling.roomName = props["Ceiling"];
        }
        

    }

    public void CustomizePrefab(GameObject prefab) {
        // Do nothing
    }
}