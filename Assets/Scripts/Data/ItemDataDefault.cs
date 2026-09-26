/* Author: Nhat Thai
 * Date: 06/01/2025
 * Description: This script initializes default item settings for all items in the game. IT IS CALLED BEFORE ANY OTHER SCRIPTS.
 *              In function GetOrCreateItem, the first variable passed is the key and the second variable is its devault stat value.
 *              Changing the default stat value in this script will imediately affect the value in the game when entering play mode or building project.
 *              
 * Brendan:
 * Sets the stat value of upgrades
 * For example: GetOrCreateItem("Backpack", X, true)
 * Backpack upgrade gives X more space
 */
using UnityEngine;

public class ItemDataDefault : MonoBehaviour
{
    private void Awake()
    {
        DataSystem.GetOrCreateItem("Backpack", 5.0f, true);
        DataSystem.GetOrCreateItem("Flashlight", 1.0f, true); 
        DataSystem.GetOrCreateItem("RunningShoe", 0.6f, true);
        DataSystem.GetOrCreateItem("Screwdriver", 0.35f, true);
        DataSystem.GetOrCreateItem("Vitamins", 20f, true);

        DataSystem.SaveData();
    }
}