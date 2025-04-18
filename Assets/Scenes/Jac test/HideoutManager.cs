using UnityEngine;

// Hideout manager will be responsible for handling placement logic
public class HideoutManager : MonoBehaviour
{
    // Object to place
    public GameObject selectedPrefab;

    public bool PlaceObject(Vector3 position)
    {
        if (selectedPrefab == null) return false;

        // instantiate and mark as placedObject
        var placedObject = Instantiate(selectedPrefab, position, Quaternion.identity);
        placedObject.tag = "PlacedObject";

        return true;
    }

}
