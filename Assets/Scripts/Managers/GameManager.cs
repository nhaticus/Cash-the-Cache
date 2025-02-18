using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int playerMoney = 0;

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
    }

    public void SpendMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
        }
    }
}

