using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomCollection
{
    public static List<RoomCollection> roomCollectionList;

    public int roomNumber;
    public BoundsInt roomBound;
    public Vector2Int roomCenter;
    public HashSet<int> roomConnections;
    public HashSet<Vector2Int> roomFloor;

    public RoomData roomData;

    /// <summary>
    /// Possible types include: "starting", "fight", "treasure", "boss", "elite_fight"
    /// </summary>
    public string roomType;

    /// <summary>
    /// Possible statuses include: "empty", "occupied", "cleared"
    /// </summary>
    public string status = "empty";

    /// <summary>
    /// Ranges from 1 - 5
    /// </summary>
    public int difficulty = 1;

    private EnemyManager enemyManager;


    public static List<string> roomTypeList = new List<string>() { "fight", "treasure", "boss", "elite_fight" };

    private List<AbstractEnemy> enemies = new List<AbstractEnemy>();

    public int wavesLeft;

    public RoomCollection(int number, BoundsInt bound, Vector2Int center, HashSet<Vector2Int> floor, EnemyManager manager)
    {
        roomNumber = number;
        roomBound = bound;
        roomCenter = center;
        roomFloor = floor;
        roomType = roomTypeList[Random.Range(0, roomTypeList.Count)];
        roomConnections = new HashSet<int>();
        enemyManager = manager;
        roomData = new RoomData(floor);
        //roomCollectionList.Add(this);
    }

    /// <summary>
    /// Creates conditionals for the fight in the room
    /// </summary>
    public void initializeFight()
    {
        Portal.toggleActive(false);
        enemies = new List<AbstractEnemy>();
        wavesLeft = Random.Range(difficulty, difficulty + 2);
        spawnNextWave();

    }


    public void checkIfAlive()
    {
        List<int> list = new List<int>();
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].health <= 0)
            {
                enemies.Remove(enemies[i]);
                i--;

            }
        }

    
    }


    /// <summary>
    /// Spawns new wave of enemies if room isn't cleared, no enemies are left, and waves are still leftover
    /// </summary>
    public void spawnNextWave()
    {
        checkIfAlive();
        if (status.Equals("cleared"))
        {
            Portal.toggleActive(true);
            return;
        }
        if (wavesLeft == 0 && enemies.Count == 0)
        {
            status = "cleared";
            return;
        }
        if (enemies.Count == 0)
        {
            if (wavesLeft > 0)
            {
                List<Vector2Int> floorList = roomFloor.ToList();
                List<Vector2Int> usedTiles = new List<Vector2Int>();
                var badGuyNumber = Random.Range(difficulty * 3, difficulty * 3 + 5);

                for (int i = 0; i < badGuyNumber; i++)
                {
                    bool done = false;
                    var position = roomCenter;
                    while (!done)
                    {
                        position = floorList[Random.Range(0, floorList.Count)];
                        if (!usedTiles.Contains(position))
                        {
                            done = true;
                        }
                    }
                    var enemy = enemyManager.CreateBaddie(position);
                    enemies.Add(enemy);

                }


                wavesLeft--;
            }
        }
    }
    public string getData()
    {
        return "RoomNumber: " + roomNumber +
        ",\n RoomBound " + roomBound +
        ",\n RoomCenter: " + "(" + roomCenter.x + ", " + roomCenter.y + ")" +
        ",\n RoomConnections: " + HashToString(roomConnections) +
        ",\n RoomFloor: it exists";
    }
    private string getRoomFloor()
    {
        string text = "[";
        foreach (var item in roomFloor) text += item + ", ";
        return text + "]";
    }

    private string HashToString(HashSet<int> hash)
    {
        string text = "[";
        foreach (var item in hash) text += item + ", ";
        return text + "]";
    }
    public void setConnections(List<HashSet<int>> wayRoomList) //lowk connections should go to wayrooms not other rooms
    {
        foreach (var wayRoom in wayRoomList)
        {
            if (wayRoom.Contains(roomNumber))
            {
                foreach (int num in wayRoom)
                {
                    if (num != roomNumber && !roomConnections.Contains(num)) roomConnections.Add(num);
                }
            }
        }
    }


}
