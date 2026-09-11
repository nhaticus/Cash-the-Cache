using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public string itemName;
    public int level;
    public float statValue;
}

[System.Serializable]
public class GameState
{
    public int currentReplay = 0;
    public int playerMoney = 0;
}

[System.Serializable]
public class GameData
{
    public GameState gameState = new();
    public List<Item> items = new();
}