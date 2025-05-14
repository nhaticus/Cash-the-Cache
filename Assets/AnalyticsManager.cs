using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Threading.Tasks;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }
    private bool isInitialized = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional if you want it to persist
        }
    }


    public async Task StartDataCollection()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();

        isInitialized = true;

        TrackGameStarted();

    }

    public void TrackGameStarted()
    {

        if (!isInitialized)
            return;

        var gameStarted = new CustomEvent("gameStarted");
        gameStarted["time"] = System.DateTime.UtcNow.ToString("o");

        AnalyticsService.Instance.RecordEvent(gameStarted);
    }

    public void LockPickingInteractionCount(int interactionCount, string safeId) {
        if (!isInitialized)
        {
            return;
        }

        var evt = new CustomEvent("lock_picking_interaction");
        evt["lock_picking_count"] = interactionCount;
        evt["safe_id"] = safeId;
        evt["time"] = System.DateTime.UtcNow.ToString("o");

        AnalyticsService.Instance.RecordEvent(evt);
        AnalyticsService.Instance.Flush();
        Debug.Log("Lock Picking Started");
    }

    public void TrackDeviceSpecs()
    {
        var deviceEvent = new CustomEvent("device_specs");
        deviceEvent["device_model"] = SystemInfo.deviceModel;
        deviceEvent["device_type"] = SystemInfo.deviceType.ToString();
        deviceEvent["os"] = SystemInfo.operatingSystem;
        deviceEvent["cpu"] = SystemInfo.processorType;
        deviceEvent["cpu_cores"] = SystemInfo.processorCount;
        deviceEvent["gpu"] = SystemInfo.graphicsDeviceName;
        deviceEvent["gpu_memory_mb"] = SystemInfo.graphicsMemorySize;
        deviceEvent["system_memory_mb"] = SystemInfo.systemMemorySize;

        AnalyticsService.Instance.RecordEvent(deviceEvent);
        Debug.Log("Device specs recorded");
    }

    public void TrackMinigameStarted(string minigameType)
    {
        var evt = new CustomEvent("minigame_started");
        evt["minigame_type"] = minigameType;
        evt["time"] = System.DateTime.UtcNow.ToString("o");

        AnalyticsService.Instance.RecordEvent(evt);
        AnalyticsService.Instance.Flush();
    }


    public void RestartGame() {
        AnalyticsService.Instance.RecordEvent("restart_game");
        AnalyticsService.Instance.Flush();
    }
}
