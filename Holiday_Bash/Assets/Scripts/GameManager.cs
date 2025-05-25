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

    private List<RoomCollection> roomCollectionsList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dungeon.GenerateDungeon();
        var startingPosition = getStartingPosition(dungeon.roomBoundryList);
        player.transform.position = new Vector3(startingPosition.x, startingPosition.y, 0);
        //Debug.Log(dungeon.getRoomsBoundries());
        roomCollectionsList = RoomCollection.roomCollectionList;

        string text = "[ \n\n";
        foreach (var collect in roomCollectionsList) {
            text += collect.getData() + "\n\n";
        }
        text += "]";
        Debug.Log(text);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public Vector2Int getStartingPosition(List<BoundsInt> rooms) {
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        List<BoundsInt> tempRoomList = new List<BoundsInt>();

        Vector2Int start;
        foreach (var room in rooms)
        {
            start = (Vector2Int)Vector3Int.RoundToInt(room.center);
            tempRoomList.Add(room);
            roomCenters.Add(start);
        }
        int randInt = Random.Range(0, roomCenters.Count);
        start = roomCenters[randInt];


        return start;
    }
}
