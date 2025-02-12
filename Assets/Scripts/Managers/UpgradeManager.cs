using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    // CREATING A SINGLETON
    public static UpgradeManager Instance;

    [SerializeField]
    private float moveSpeedUpgradeIncrement = 0.5f;

    [SerializeField]
    private int maxWeightUpgradeIncrement = 3;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void upgradeSpeed()
    {
        PlayerManager.Instance.increaseMoveSpeed(moveSpeedUpgradeIncrement);
    }

    public void upgradeMaxWeight()
    {
        PlayerManager.Instance.increaseMaxWeight(maxWeightUpgradeIncrement);
    }
}
