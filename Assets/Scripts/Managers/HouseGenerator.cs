using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HouseGenerator : MonoBehaviour
{
    public GameObject floorTilePrefab;
    public GameObject wallTilePrefab;
    public GameObject doorTilePrefab;
    public GameObject roomLabelPrefab; 

    public int gridSizeX = 100;
    public int gridSizeY = 100;

    [System.Serializable]
    public class RoomType
    {
        public string name;
    }

    public List<RoomType> roomTypes = new List<RoomType>() {
        new RoomType(){ name = "Living Room" },
        new RoomType(){ name = "Kitchen" },
        new RoomType(){ name = "Bedroom" },
        new RoomType(){ name = "Bathroom" },
        new RoomType(){ name = "Office" },
        new RoomType(){ name = "Dining Room" }
    };

    private int[,] grid;
    private List<Color> roomColors = new List<Color>();
    private List<Vector2> roomCenters = new List<Vector2>();

    void Start()
    {
        grid = new int[gridSizeX, gridSizeY];
        GenerateHouse();
    }
    void GenerateHouse()
    {
        int roomID = 1;
        int attempts = 0;
        int maxAttempts = roomTypes.Count * 10;
        int i = 0;

        while (i < roomTypes.Count && attempts < maxAttempts)
        {
            int roomWidth = Random.Range(4, 7);
            int roomHeight = Random.Range(4, 7);
            int xStart = Random.Range(2, gridSizeX - roomWidth - 2);
            int yStart = Random.Range(2, gridSizeY - roomHeight - 2);

            List<Vector2Int> carvedRoom = GenerateRoomOutline(xStart, yStart, roomWidth, roomHeight);
            if (carvedRoom.Count < 4) { attempts++; continue; }

            Color roomColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.7f, 1f);
            roomColors.Add(roomColor);

            Vector2 center = Vector2.zero;

            foreach (var pos in carvedRoom)
            {
                grid[pos.x, pos.y] = roomID;
                GameObject floor = Instantiate(floorTilePrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
                floor.GetComponent<SpriteRenderer>().color = roomColor;
                center += pos;
            }

            center /= carvedRoom.Count;
            roomCenters.Add(center);
            PlaceRoomLabel(roomTypes[i].name, center);
            GenerateWallsAndDoorsAroundRoom(carvedRoom);

            roomID++;
            i++;
            attempts++;
        }

        ConnectRooms();
    }
    List<Vector2Int> GenerateRoomOutline(int xStart, int yStart, int width, int height)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();

        // Start with full rectangle
        for (int x = xStart; x < xStart + width; x++)
        {
            for (int y = yStart; y < yStart + height; y++)
            {
                tiles.Add(new Vector2Int(x, y));
            }
        }

        // Carve out some corners or side segments (indents)
        int indentAttempts = Random.Range(1, 4); // 1–3 indents
        for (int i = 0; i < indentAttempts; i++)
        {
            int side = Random.Range(0, 4);
            int carveWidth = Random.Range(1, width / 2);
            int carveHeight = Random.Range(1, height / 2);

            List<Vector2Int> cutout = new List<Vector2Int>();

            switch (side)
            {
                case 0: // Top-left
                    for (int x = xStart; x < xStart + carveWidth; x++)
                        for (int y = yStart + height - carveHeight; y < yStart + height; y++)
                            cutout.Add(new Vector2Int(x, y));
                    break;
                case 1: // Top-right
                    for (int x = xStart + width - carveWidth; x < xStart + width; x++)
                        for (int y = yStart + height - carveHeight; y < yStart + height; y++)
                            cutout.Add(new Vector2Int(x, y));
                    break;
                case 2: // Bottom-left
                    for (int x = xStart; x < xStart + carveWidth; x++)
                        for (int y = yStart; y < yStart + carveHeight; y++)
                            cutout.Add(new Vector2Int(x, y));
                    break;
                case 3: // Bottom-right
                    for (int x = xStart + width - carveWidth; x < xStart + width; x++)
                        for (int y = yStart; y < yStart + carveHeight; y++)
                            cutout.Add(new Vector2Int(x, y));
                    break;
            }

            foreach (var cell in cutout)
                tiles.Remove(cell);
        }

        return tiles;
    }


    void ConnectRooms()
    {
        for (int i = 0; i < roomCenters.Count - 1; i++)
        {
            Vector2 a = roomCenters[i];
            Vector2 b = roomCenters[i + 1];

            Vector2Int posA = Vector2Int.RoundToInt(a);
            Vector2Int posB = Vector2Int.RoundToInt(b);

            // L-shaped hallway
            for (int x = Mathf.Min(posA.x, posB.x); x <= Mathf.Max(posA.x, posB.x); x++)
            {
                PlaceHallwayTile(x, posA.y);
            }
            for (int y = Mathf.Min(posA.y, posB.y); y <= Mathf.Max(posA.y, posB.y); y++)
            {
                PlaceHallwayTile(posB.x, y);
            }
        }
    }

    void PlaceHallwayTile(int x, int y)
    {
        if (grid[x, y] == 0)
        {
            grid[x, y] = -1; // Mark as hallway
            Instantiate(floorTilePrefab, new Vector3(x, y, 0), Quaternion.identity);
        }

        // Replace walls with door if surrounded
        Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(x, y), 0.1f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Wall"))
            {
                Destroy(hit.gameObject);
                Instantiate(doorTilePrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }




    bool RoomOverlaps(int xStart, int yStart, int width, int height)
    {
        for (int x = xStart - 1; x <= xStart + width; x++)
        {
            for (int y = yStart - 1; y <= yStart + height; y++)
            {
                if (x >= 0 && y >= 0 && x < gridSizeX && y < gridSizeY)
                {
                    if (grid[x, y] != 0)
                        return true;
                }
            }
        }
        return false;
    }

    void GenerateWallsAndDoorsAroundRoom(List<Vector2Int> roomTiles)
    {
        foreach (var tile in roomTiles)
        {
            foreach (var dir in new Vector2Int[] {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int neighbor = tile + dir;
                if (InBounds(neighbor.x, neighbor.y) && grid[neighbor.x, neighbor.y] == 0)
                {
                    Quaternion rot = (dir == Vector2Int.left || dir == Vector2Int.right) ?
                        Quaternion.Euler(0, 0, 90) : Quaternion.identity;

                    Instantiate(wallTilePrefab, new Vector3(neighbor.x, neighbor.y, 0), rot);
                }
            }
        }
    }


    void PlaceRoomLabel(string label, Vector2 position)
    {
        GameObject labelObj = Instantiate(roomLabelPrefab, new Vector3(position.x, position.y, -0.1f), Quaternion.identity);
        TMP_Text text = labelObj.GetComponent<TMP_Text>();
        if (text != null)
        {
            text.text = label;
            text.fontSize = 5f;
            text.color = Color.black;
        }
    }

    bool InBounds(int x, int y)
    {
        return x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY;
    }
}
