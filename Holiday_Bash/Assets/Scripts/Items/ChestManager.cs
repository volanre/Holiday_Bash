using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChestManager : MonoBehaviour
{
    public TreasureChest commonChest;
    public TreasureChest eliteChest;
    public TreasureChest bossChest;
    [NonSerialized] public Player player;
    public void SpawnChest(RoomCollection room)
    {

        List<Vector2Int> floorList = room.roomFloor.ToList();
        HashSet<Vector2Int> usedTiles = new HashSet<Vector2Int>();
        usedTiles.UnionWith(room.propPositions);


        bool done = false;
        Vector2Int position = room.roomCenter + (2*Vector2Int.up);

        if (!usedTiles.Contains(position) && room.accessiblePaths.Contains(position)) //default chest position is above spawn
        {
            done = true;
        }

        while (!done)
        {
            position = room.accessiblePaths[Random.Range(0, room.accessiblePaths.Count)];
            if (!usedTiles.Contains(position))
            {
                done = true;
            }
        }
        Vector3 newPos = new Vector3(position.x, position.y, 0);
        var chest = Instantiate(commonChest, newPos, Quaternion.identity);
        chest.room = room;
        chest.player = player;
        Debug.Log("Chest spawned at: " + newPos + ", player at: " + player.getPosition());
    }
}
