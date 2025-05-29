using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using Mono.Cecil.Cil;
using NUnit.Framework.Constraints;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 35, minRoomHeight = 35; //these min sizes are inclusive of offset

    [SerializeField]
    private int maxRoomWidth = 50, maxRoomHeight = 50; //these min sizes are inclusive of offset

    [SerializeField]
    public int dungeonWidth = 500, dungeonHeight = 500;

    [SerializeField]
    private int maxRoomCount = 20, minRoomCount = 12;

    [SerializeField]
    [Range(0, 20)]
    private int offset = 10;

    [SerializeField]
    private EnemyManager enemyManager;

    public List<HashSet<int>> wayRoomList;


    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        WallGenerator.resetWallPositions();
        Portal.resetPortalList();


        var roomBoundryList = ProceduralGenerationAlgorithms.PersonalGridSpaceGeneration(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), maxRoomWidth, maxRoomHeight, maxRoomCount, minRoomCount);
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        var roomLayoutList = CreateSimpleRooms(roomBoundryList, minRoomWidth, minRoomHeight, maxRoomWidth, maxRoomHeight);

        foreach (var room in roomLayoutList) floor.UnionWith(room);
        CreateRoomCollection(roomBoundryList, roomLayoutList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomBoundryList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        var corridorHash = ConnectRooms(roomCenters, roomBoundryList);

        //Create roomlinks and set path inside each room
        foreach (var roomColect in RoomCollection.roomCollectionList)//
        {
            roomColect.setConnections(wayRoomList);
            roomColect.roomData.assignRoomPathing(roomColect.roomFloor, corridorHash);
            roomColect.findAccessiblePaths();
        }

        //creates indiviudal roomlayout
        tilemapVisualizer.paintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        //adds corridors and gates
        floor.UnionWith(corridorHash);
        tilemapVisualizer.paintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer); //create method to toggle on and off gates

        WayroomGenerator.CreateWayRoomBounds(dungeonWidth, dungeonHeight, WayroomGenerator.roomSize, wayRoomList, tilemapVisualizer);
        var wayroomLayountList = CreateSimpleRooms(WayroomGenerator.wayroomBoundsList,WayroomGenerator.roomSize,WayroomGenerator.roomSize,WayroomGenerator.roomSize,WayroomGenerator.roomSize);
        //Debug.Log(wayroomLayountList.Count);
        WayroomGenerator.CreateWayRooms(wayroomLayountList, tilemapVisualizer, wayRoomList);
    }

    private void CreateRoomCollection(List<BoundsInt> roomBoundryList, List<HashSet<Vector2Int>> roomLayouts)
    {
        RoomCollection.roomCollectionList = new List<RoomCollection>();
        List<BoundsInt> tempRoomList = new List<BoundsInt>();

        foreach (var room in roomBoundryList)
        {
            tempRoomList.Add(room);
        }


        for (int i = 0; i < tempRoomList.Count; i++)
        {
            var thisCollection = new RoomCollection(i, tempRoomList[i], (Vector2Int)Vector3Int.RoundToInt(tempRoomList[i].center), roomLayouts[i], enemyManager);
            // thisCollection.roomNumber = i;
            // thisCollection.roomBound = tempRoomList[i];
            // thisCollection.roomCenter = (Vector2Int)Vector3Int.RoundToInt(tempRoomList[i].center);
            // thisCollection.roomConnections = new HashSet<int>();
            // thisCollection.roomFloor = roomLayouts[i];
            // thisCollection.roomType = RoomCollection.roomTypeList[Random.Range(0,RoomCollection.roomTypeList.Count)];
            RoomCollection.roomCollectionList.Add(thisCollection);
        }
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters, List<BoundsInt> partitons)
    {
        List<HashSet<Vector2Int>> corridors = new List<HashSet<Vector2Int>>();
        HashSet<Vector2Int> realCorridorHash = new HashSet<Vector2Int>();
        List<List<int>> connectedRooms = new List<List<int>>();
        int currentRoom = 0;
        // var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        // roomCenters.Remove(currentRoomCenter);
        List<int> usedRooms = new List<int>();
        usedRooms.Add(currentRoom);
        int index = 0;
        while (usedRooms.Count < RoomCollection.roomCollectionList.Count)
        {
            
            HashSet<Vector2Int> currentCorridor = new HashSet<Vector2Int>();
            List<int> currentConnectedRooms = new List<int>();

            var currentRoomCenter = RoomCollection.roomCollectionList[currentRoom].roomCenter;
            int closestRoomNumber = FindClosestPoint(currentRoomCenter, RoomCollection.roomCollectionList, usedRooms);
            Vector2Int partitionMidpoint = FindPartitionMidpoint(currentRoomCenter, RoomCollection.roomCollectionList[closestRoomNumber].roomCenter, partitons);
            currentCorridor.UnionWith(CreateCorridor(currentRoomCenter, partitionMidpoint));
            currentCorridor.UnionWith(CreateCorridor(partitionMidpoint, RoomCollection.roomCollectionList[closestRoomNumber].roomCenter));
            currentConnectedRooms.Add(currentRoom);
            currentConnectedRooms.Add(closestRoomNumber);

            realCorridorHash.UnionWith(currentCorridor);

            corridors.Add(currentCorridor);
            connectedRooms.Add(currentConnectedRooms);

            currentRoom = closestRoomNumber;
            usedRooms.Add(currentRoom);
            // roomCollectionList[currentRoom].roomConnections.Add(closestRoomNumber);
            index++;
        }

        //Wayroom Generation
        WayroomGenerator.wayroomCorridorList = new List<HashSet<Vector2Int>>();
        wayRoomList = new List<HashSet<int>>(); 
        List<int> usedCorridors = new List<int>();
        HashSet<Vector2Int> roomfloor = new HashSet<Vector2Int>();
        foreach (var room in RoomCollection.roomCollectionList) roomfloor.UnionWith(room.roomFloor);
        for (int i = 0; i < corridors.Count; i++)
        {
            if (usedCorridors.Contains(i)) continue;
            HashSet<int> currentWayRoom = new HashSet<int>();
            currentWayRoom.UnionWith(connectedRooms[i]);
            var currentWayroomCorridorfloor = new HashSet<Vector2Int>();
            currentWayroomCorridorfloor.UnionWith(corridors[i]);
            usedCorridors.Add(i);
            for (int p = 0; p < corridors.Count; p++)
            {
                if (usedCorridors.Contains(p)) continue;
                if (corridors[p].Intersect(corridors[i]).Count() > 0)
                {
                    if (corridors[p].Intersect(corridors[i]).Except(roomfloor).Count() > 0)
                    {
                        usedCorridors.Add(p);
                        currentWayroomCorridorfloor.UnionWith(corridors[p]);
                        currentWayRoom.UnionWith(connectedRooms[p]);
                    }
                }
            }
            WayroomGenerator.wayroomCorridorList.Add(currentWayroomCorridorfloor);
            wayRoomList.Add(currentWayRoom);
        }

        // while (roomCenters.Count > 0)
        // {
        //     Vector2Int closest = FindClosestPoint(currentRoomCenter, roomCenters);

        //     //Find Partition Midpoint
        //     Vector2Int partitionMidpoint = FindPartitionMidpoint(currentRoomCenter, closest, partitons);

        //     // HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);

        //     //Corridor from RoomA center to midpoint, then from midpoint to roomB center;
        //     corridors.UnionWith(CreateCorridor(currentRoomCenter, partitionMidpoint));
        //     corridors.UnionWith(CreateCorridor(partitionMidpoint, closest));

        //     currentRoomCenter = closest;
        //     roomCenters.Remove(closest);
        // }



        return realCorridorHash;
    }


    private Vector2Int FindPartitionMidpoint(Vector2Int roomA, Vector2Int roomB, List<BoundsInt> partitions)
    {
        foreach (var partition in partitions)
        {
            // Check if both room centers lie inside this partition
            if (partition.xMin <= roomA.x && roomA.x < partition.xMax &&
                partition.yMin <= roomA.y && roomA.y < partition.yMax &&
                partition.xMin <= roomB.x && roomB.x < partition.xMax &&
                partition.yMin <= roomB.y && roomB.y < partition.yMax)
            {
                return (Vector2Int)Vector3Int.RoundToInt(partition.center);
            }
        }
        // Fallback: return midpoint between rooms if no partition contains both
        return (roomA + roomB) / 2;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y) position += Vector2Int.up;
            else if (destination.y < position.y) position += Vector2Int.down;
            corridor.Add(position);
            corridor.Add(position + Vector2Int.left);
            corridor.Add(position + Vector2Int.right);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x) position += Vector2Int.right;
            else if (destination.x < position.x) position += Vector2Int.left;
            corridor.Add(position);
            corridor.Add(position + Vector2Int.up);
            corridor.Add(position + Vector2Int.down);
        }


        return corridor;
    }
    private int FindClosestPoint(Vector2Int currentRoomCenter, List<RoomCollection> roomCollectionList, List<int> usedRooms)
    {
        Vector2Int closest = Vector2Int.zero;
        int closestRoomNumber = 0;
        float distance = float.MaxValue;
        for(int i = 0; i < roomCollectionList.Count; i++)
        {
            if (usedRooms.Contains(i)) continue;
            var thisCenter = roomCollectionList[i].roomCenter;
            float currentDistance = Vector2.Distance(thisCenter, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closestRoomNumber = i;
            }
        }
        return closestRoomNumber;
    }
    // private Vector2Int FindClosestPoint(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    // {
    //     Vector2Int closest = Vector2Int.zero;
    //     float distance = float.MaxValue;
    //     foreach (var position in roomCenters)
    //     {
    //         float currentDistance = Vector2.Distance(position, currentRoomCenter);
    //         if (currentDistance < distance)
    //         {
    //             distance = currentDistance;
    //             closest = position;
    //         }
    //     }
    //     return closest;
    // }

    private List<HashSet<Vector2Int>> CreateSimpleRooms(List<BoundsInt> roomsList, int minRoomWidth, int minRoomHeight, int maxRoomWidth, int maxRoomHeight)
    {
        List<HashSet<Vector2Int>> hashedRoomsList = new List<HashSet<Vector2Int>>(); //IMPLEMENT LATER
        foreach (var room in roomsList)
        {
            int randWidth = Random.Range(minRoomWidth, maxRoomWidth) - 2 * offset;
            int randHeight = Random.Range(minRoomHeight, maxRoomHeight) - 2 * offset;

            int xOff = (room.size.x - (2 * offset) - randWidth) / 2;
            int yOff = (room.size.y - (2 * offset) - randHeight) / 2;

            HashSet<Vector2Int> currentRoom = new HashSet<Vector2Int>();
            for (int col = offset + xOff; col < room.size.x - offset - xOff; col++)
            {
                for (int row = offset + yOff; row < room.size.y - offset - yOff; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    currentRoom.Add(position);
                }
            }
            hashedRoomsList.Add(currentRoom);
        }

        return hashedRoomsList;

    }
    /// <summary>
    /// Returns a list all rooms.
    /// Each room is a hashset of the positions of the floortiles
    /// </summary>
    /// <returns>List of Rooms</returns>
    // public List<HashSet<Vector2Int>> getRoomsPositions()
    // {
    //     return roomLayoutList;
    // }
    // public List<BoundsInt> getRoomsBoundries()
    // {
    //     return roomBoundryList;
    // }

    // public HashSet<Vector2Int> getCorridors()
    // {
    //     return corridorHash;
    // }
    
}
