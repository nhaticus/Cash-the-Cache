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
 * 
 * Used to set upgrade values and how much many days until game end and money you need to win
 */
using UnityEngine;

public class ItemDataDefault : MonoBehaviour
{
    private void Awake()
    {
        DataSystem.GetOrCreateItem("Backpack", 5.0f, true);
        DataSystem.GetOrCreateItem("Flashlight", 1.0f, true); 
        DataSystem.GetOrCreateItem("RunningShoe", 0.6f, true);
        DataSystem.GetOrCreateItem("Screwdriver", 1f, true);
        DataSystem.GetOrCreateItem("Vitamins", 20f, true);

        DataSystem.GetOrCreateItem("DaysTotal", 45, true);
        DataSystem.GetOrCreateItem("MoneyToEnd", 10000, true);

        DataSystem.SaveData();
    }
}