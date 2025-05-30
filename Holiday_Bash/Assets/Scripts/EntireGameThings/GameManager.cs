using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private Player player;
    [SerializeField]
    private RoomFirstDungeonGenerator dungeon;

    [SerializeField]
    private PropPlacementManager propPlacer;

    private int FloorOfTheDungeon = 1;
    [Header("Settings")]
    [SerializeField,Range(0,10)] public static float SFXVolume = 1;

    void Start()
    {
        dungeon.GenerateDungeon();
        propPlacer.ProcessRooms();
        var startingPosition = setStartingPosition();
        setBossRoom();
        //player.transform.position = new Vector3(startingPosition.x, startingPosition.y, 0);

        string text = "[ \n\n";
        foreach (var collect in RoomCollection.roomCollectionList)
        {
            text += collect.getData() + "\n\n";
        }
        text += "]";
        Debug.Log(text);
    }

    void Update()
    {
        foreach (var room in RoomCollection.roomCollectionList)
        {
            if (room.roomBound.Contains(player.getPosition()))
            {

                if (room.status.Equals("empty"))
                {
                    Debug.Log("player is in this room: " + room.roomCenter);
                    if (room.roomType.Equals("fight"))
                    {
                        room.initializeFight();
                    }
                    else if (room.roomType.Equals("boss"))
                    {
                        room.startBossFight(FloorOfTheDungeon);
                    }
                    else
                    {
                        Portal.toggleActive(true);
                    }
                    room.status = "occupied";

                }

                room.spawnNextWave();

            }
        }
    }
    public Vector2Int setStartingPosition() //redo this to not choose the boss room as the starting room
    {

        int randInt = Random.Range(0, RoomCollection.roomCollectionList.Count);
        Vector2Int start = RoomCollection.roomCollectionList[randInt].roomCenter;
        RoomCollection.roomCollectionList[randInt].roomType = "starting";

        return start;
    }

    //move this to the roomfirst dungeon generator to make it BEFORE wayrooms get generated
    //that way when painting wayroom floors can paint the correct boosroom tile
    public void setBossRoom()
    {
        int randInt = Random.Range(0, RoomCollection.roomCollectionList.Count);
        if (RoomCollection.roomCollectionList[randInt].roomType == "fight")
        {
            RoomCollection.roomCollectionList[randInt].roomType = "boss";
            Debug.Log("Boss Room Number: " + RoomCollection.roomCollectionList[randInt].roomNumber);
            var tempCenter = RoomCollection.roomCollectionList[randInt].roomCenter;
            player.transform.position = new Vector3(tempCenter.x, tempCenter.y, 0);
        }
        else
        {
            setBossRoom();
        }
    }
}
