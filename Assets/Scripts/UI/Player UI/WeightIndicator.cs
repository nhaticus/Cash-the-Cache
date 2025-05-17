using TMPro;
using UnityEngine;

/*
 * Object attached under Player UI crosshair
 * Displays the weight of a selected Stealable Object
 */

public class WeightIndicator : MonoBehaviour
{
    [SerializeField] GameObject weightTextObj;
    TMP_Text weightText;

    [SerializeField] Color normalColor = Color.white, heavyColor = Color.red;

    PlayerInteract playerInteract;

    void Start()
    {
        weightTextObj.SetActive(false);
        weightText = weightTextObj.GetComponent<TMP_Text>();
    }

    public void Initialize(GameObject _player)
    {
        playerInteract = _player.GetComponentInChildren<PlayerInteract>();
        playerInteract.ObjectSelected.AddListener(GetPlayerObject);
    }

    /// <summary>
    /// Handles the player's ObjectSelected event.
    /// When passed a GameObject, it sends its StealableObject information to SetWeightText().
    /// </summary>
    /// <param name="obj"></param>
    void GetPlayerObject(GameObject obj)
    {
        if (obj == null)
        {
            weightTextObj.SetActive(false);
            return;
        }
        StealableObject stealableObject = obj.GetComponent<StealableObject>();
        if (stealableObject)
            SetWeightText(stealableObject);
    }

    /// <summary>
    /// Sets the weight text.
    /// Changes text to red if too heavy.
    /// </summary>
    /// <param name="so"></param>
    void SetWeightText(StealableObject so)
    {
        weightTextObj.SetActive(true);
        weightText.text = so.lootInfo.weight.ToString();
        int weightLimit = PlayerManager.Instance.getMaxWeight() - PlayerManager.Instance.getWeight();
        if (so.lootInfo.weight > weightLimit)
            weightText.color = heavyColor;
        else
            weightText.color = normalColor;
    }
}
