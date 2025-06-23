using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpecialPortal : MonoBehaviour
{
    [NonSerialized] public Player player;
    [NonSerialized] public static TilemapVisualizer tilemapVisualizer; 
    private Vector3 destination;
    public bool playerInside = false;
    private bool teleportTriggered = false;
    [NonSerialized] public EnemyManager enemyManager;
    void Update()
    {
        if (playerInside)
        {
            if (!teleportTriggered && player.GetInteraction())
            {
                teleportTriggered = true;
                Debug.Log("buttonpressedoinside");
                Teleport();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
    
    public void Teleport()
    {
        int locationX = -100;
        int locationY = 100;
        int offset = 1;
        BoundsInt room = new BoundsInt(locationX, locationY, 0, 30, 30, 1);
        int randWidth = 15;
        int randHeight = 20;

        int xOff = (room.size.x - (2 * offset) - randWidth) / 2;
        int yOff = (room.size.y - (2 * offset) - randHeight) / 2;

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int col = offset + xOff; col < room.size.x - offset - xOff; col++)
        {
            for (int row = offset + yOff; row < room.size.y - offset - yOff; row++)
            {
                Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                floor.Add(position);
            }
        }


        RoomCollection thisCollection = new RoomCollection(999, room, (Vector2Int)Vector3Int.RoundToInt(room.center), floor, enemyManager);
        thisCollection.roomType = "special";
        thisCollection.roomName = "IfritRoom";
        thisCollection.findAccessiblePaths();
        if (tilemapVisualizer == null)
        {
            Debug.Log("f;ousb;uab");
            return;
        }

        tilemapVisualizer.paintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        RoomCollection.roomCollectionList.Add(thisCollection);
        destination = room.center;
        Vector3 place = new Vector3(destination.x, destination.y);
        player.transform.position = place;
    }
}
