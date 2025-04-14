using UnityEngine;

// Hideout manager will be responsible for handling placement logic
public class HideoutManager : MonoBehaviour
{
    // Reference to the object prefab the player wants to place
    public GameObject selectedPrefab;

    // Call when the player wants to place an object
    public void PlaceObject(Vector3 position)
    {
        if (selectedPrefab != null)
        {
            // Instantiate the object at the desired position with default rotation
            GameObject placedObject = Instantiate(selectedPrefab, position, Quaternion.identity);
            
        }
    }
}
