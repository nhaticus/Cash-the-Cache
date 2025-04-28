using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int playerMoney;

    public event Action OnNPCLeaving;

    private Vector3 NPCExitPoint; // Exit for NPCs to leave the map

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try to find the exit point if it exists in the scene
        GameObject exitObj = GameObject.FindWithTag("ExitPoint");
        if (exitObj != null)
        {
            NPCExitPoint = exitObj.transform.position;
        }
        else
        {
            NPCExitPoint = Vector3.zero;
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

    public Vector3 GetNPCExitPoint()
    {
        return NPCExitPoint;
    }

    public void NPCLeaving()
    {
        OnNPCLeaving?.Invoke();
    }
}

