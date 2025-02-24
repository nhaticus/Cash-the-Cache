/* Attached to each UI shop item prefab to access the text components
 */
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemTemplate : MonoBehaviour
{
    public Item itemData;
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public TMP_Text itemLevel;
    public TMP_Text itemStats;
    public TMP_Text itemPrice;
    public Button buyButton;

}
