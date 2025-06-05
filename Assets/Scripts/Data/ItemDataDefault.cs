/* Author: Nhat Thai
 * Date: 06/01/2025
 * Description: This script initializes default item settings for all items in the game. IT IS CALLED BEFORE ANY OTHER SCRIPTS.
 *              In function GetOrCreateItem, the first variable passed is the key and the second variable is its devault stat value.
 *              Changing the default stat value in this script will imediately affect the value in the game when entering play mode or building project.
*/
using UnityEngine;

public class ItemDataDefault : MonoBehaviour
{
    private void Awake()
    {
        DataSystem.GetOrCreateItem("Backpack", 10.0f, true);
        DataSystem.GetOrCreateItem("Flashlight", 1.0f, true); 
        DataSystem.GetOrCreateItem("RunningShoe", 0.5f, true);
        DataSystem.GetOrCreateItem("Screwdriver", 0.3f, true);

        DataSystem.SaveData();
    }
}