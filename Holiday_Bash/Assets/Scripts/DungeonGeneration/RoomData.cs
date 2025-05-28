using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public HashSet<Vector2Int> floorNearLeftWall = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> floorNearRightWall = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> floorNearDownWall = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> floorNearUpWall = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> floorNearCorner = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> floorInsideMiddle = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> floorInPath = new HashSet<Vector2Int>();

    public RoomData(HashSet<Vector2Int> floorTiles)
    {
        foreach (var tile in floorTiles)
        {
            int neighborCount = 4;
            if (!floorTiles.Contains(tile + Vector2Int.up))
            {
                floorNearUpWall.Add(tile);
                neighborCount--;
            }
            if (!floorTiles.Contains(tile + Vector2Int.down))
            {
                floorNearDownWall.Add(tile);
                neighborCount--;
            }
            if (!floorTiles.Contains(tile + Vector2Int.left))
            {
                floorNearLeftWall.Add(tile);
                neighborCount--;
            }
            if (!floorTiles.Contains(tile + Vector2Int.right))
            {
                floorNearRightWall.Add(tile);
                neighborCount--;
            }

            if (neighborCount == 2)
            {
                floorNearCorner.Add(tile);
            }
            else if (neighborCount == 4)
            {
                floorInsideMiddle.Add(tile);
            }

            floorNearDownWall.ExceptWith(floorNearCorner);
            floorNearUpWall.ExceptWith(floorNearCorner);
            floorNearLeftWall.ExceptWith(floorNearCorner);
            floorNearRightWall.ExceptWith(floorNearCorner);
        }
    }

    public void assignRoomPathing(HashSet<Vector2Int> thisRoomFloorHashing, HashSet<Vector2Int> corridorHashing)
    {
        foreach (var tile in corridorHashing)
        {
            if (thisRoomFloorHashing.Contains(tile))
            {
                floorInPath.Add(tile);
            }
        }
        
    }
}
    
