using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int playerMoney;

    public event Action OnNPCLeaving;

    public int numRuns = 0; // number of times replayed to reduce max time
                            // Shop Van increases when go to level

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object between scenes
        }
        else
        {
            Destroy(gameObject);
        }
        playerMoney = PlayerPrefs.GetInt("Money", 0);
    }
    public enum GameState
    {
        Play,
        Pause,
        Over
    }

    public GameState CurrentState { get; private set; }

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
    }
    public void AddMoney(int amount)
    {
        playerMoney += amount;
        PlayerPrefs.SetInt("Money", playerMoney);
    }

    public void SpendMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            PlayerPrefs.SetInt("Money", playerMoney);
        }
    }
    
    public void NPCLeaving()
    {
        OnNPCLeaving?.Invoke();
    }
}

