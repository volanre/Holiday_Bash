using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    private List<Vector2Int> accessiblePaths;


    public static List<string> roomTypeList = new List<string>() { "fight", "treasure", "boss", "elite_fight" };

    private List<AbstractEnemy> enemies = new List<AbstractEnemy>();

    public int wavesLeft;
    public HashSet<Vector2Int> propPositions;
    public List<GameObject> propReferences;

    public RoomCollection(int number, BoundsInt bound, Vector2Int center, HashSet<Vector2Int> floor, EnemyManager manager)
    {
        roomNumber = number;
        roomBound = bound;
        roomCenter = center;
        roomFloor = floor;
        //roomType = roomTypeList[Random.Range(0, roomTypeList.Count)]; //turn this back on later
        roomType = roomTypeList[0]; //sets all of them to fight type rooms
        roomConnections = new HashSet<int>();
        enemyManager = manager;
        propPositions = new HashSet<Vector2Int>();
        propReferences = new List<GameObject>();
        roomData = new RoomData(floor); //call the roomdata pathing function 
        //roomCollectionList.Add(this);
    }

    public void findAccessiblePaths()
    {
        RoomGraph roomGraph = new RoomGraph(roomFloor);
        HashSet<Vector2Int> newFloor = new HashSet<Vector2Int>(roomFloor);
        newFloor.IntersectWith(roomData.floorInPath);

        Dictionary<Vector2Int, Vector2Int> roomMap = roomGraph.RunBFS(roomFloor.First(), propPositions);
        accessiblePaths = roomMap.Keys.OrderBy(x => Guid.NewGuid()).ToList();
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

    public void CheckEnemiesRemaining()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].health <= 0)
            {
                enemies.Remove(enemies[i]);
                i--;
            }
        }
    }

    public void startBossFight(int FloorOfTheDungeon)
    {
        Portal.toggleActive(false);
        enemies = new List<AbstractEnemy>();
        var position = accessiblePaths[Random.Range(0, accessiblePaths.Count)];
        var boss = enemyManager.CreateBaddie(position, FloorOfTheDungeon, true);
        boss.room = this;
        enemies.Add(boss);
    }

    /// <summary>
    /// Spawns new wave of enemies if room isn't cleared, no enemies are left, and waves are still leftover
    /// </summary>
    public void spawnNextWave()
    {
        CheckEnemiesRemaining();
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
                HashSet<Vector2Int> usedTiles = new HashSet<Vector2Int>();
                usedTiles.UnionWith(propPositions);
                var badGuyNumber = Random.Range(difficulty * 3, difficulty * 3 + 5);

                for (int i = 0; i < badGuyNumber; i++)
                {
                    bool done = false;
                    Vector2Int position = roomCenter;
                    while (!done)
                    {
                        position = accessiblePaths[Random.Range(0, accessiblePaths.Count)];
                        if (!usedTiles.Contains(position))
                        {
                            done = true;
                        }
                    }
                    var enemy = enemyManager.CreateBaddie(position);
                    enemy.room = this;
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

public class RoomGraph
{
    Dictionary<Vector2Int, List<Vector2Int>> graph = new Dictionary<Vector2Int, List<Vector2Int>>();
    public RoomGraph(HashSet<Vector2Int> flooring)
    {
        foreach (var pos in flooring)
        {
            List<Vector2Int> neightbors = new List<Vector2Int>();
            foreach (Vector2Int dir in Direction2D.cardinalDirectionsList)
            {
                Vector2Int newPos = pos + dir;
                if (flooring.Contains(newPos))
                {
                    neightbors.Add(newPos);
                }
            }
            graph.Add(pos, neightbors);
        }
    }
    public Dictionary<Vector2Int, Vector2Int> RunBFS(Vector2Int startPos, HashSet<Vector2Int> occupiedNodes)
    {
        Queue<Vector2Int> nodesToVisit = new Queue<Vector2Int>();
        nodesToVisit.Enqueue(startPos);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>();
        visitedNodes.Add(startPos);

        Dictionary<Vector2Int, Vector2Int> map = new Dictionary<Vector2Int, Vector2Int>();
        map.Add(startPos, startPos);
        while (nodesToVisit.Count > 0)
        {
            Vector2Int node = nodesToVisit.Dequeue();
            List<Vector2Int> neighbors = graph[node];
            foreach (Vector2Int pos in neighbors)
            {
                if (!visitedNodes.Contains(pos) && !occupiedNodes.Contains(pos))
                {
                    nodesToVisit.Enqueue(pos);
                    visitedNodes.Add(pos);
                    map[pos] = node;
                }
            }
        }
        return map;
        
    }
}
