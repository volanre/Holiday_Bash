using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    public Player player;
    [SerializeField]
    public RoomFirstDungeonGenerator dungeon;

    void Start()
    {
        dungeon.GenerateDungeon();
        var startingPosition = getStartingPosition();
        player.transform.position = new Vector3(startingPosition.x, startingPosition.y, 0);

        string text = "[ \n\n";
        foreach (var collect in RoomCollection.roomCollectionList) {
            text += collect.getData() + "\n\n";
        }
        text += "]";
        Debug.Log(text);
    }

    void Update()
    {
        foreach(var room in RoomCollection.roomCollectionList) {
            if (room.roomBound.Contains(player.getPosition()))
            {

                if (room.status.Equals("empty"))
                {
                    Debug.Log("player is in this room: " + room.roomCenter);
                    room.initializeFight();
                    room.status = "occupied";
                    
                }

            room.spawnNextWave();
            
            }
        }
    }
    public Vector2Int getStartingPosition() {

        int randInt = Random.Range(0, RoomCollection.roomCollectionList.Count);
        Vector2Int start = RoomCollection.roomCollectionList[randInt].roomCenter;
        RoomCollection.roomCollectionList[randInt].roomType = "starting";

        return start;
    }
}
