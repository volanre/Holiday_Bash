using System;
using System.Collections.Generic;
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

    public RoomCollection(int number, BoundsInt bound, Vector2Int center, HashSet<Vector2Int> floor)
    {
        roomNumber = number;
        roomBound = bound;
        roomCenter = center;
        roomFloor = floor;
        roomType = roomTypeList[Random.Range(0, roomTypeList.Count)];
        roomConnections = new HashSet<int>();
        
        //roomCollectionList.Add(this);

    }

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


    public static List<string> roomTypeList = new List<string>() { "fight", "treasure", "boss", "elite_fight" };

    public string getData()
    {
        return "RoomNumber: " + roomNumber +
        ",\n RoomBound " + roomBound +
        ",\n RoomCenter: " + "(" + roomCenter.x + ", " + roomCenter.y + ")" +
        ",\n RoomConnections: " + HashToString(roomConnections) +
        ",\n RoomFloor: it exists";
    }
    public string getRoomFloor()
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
