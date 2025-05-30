using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WayroomGenerator
{
    public static int roomSize = 29;
    public static List<BoundsInt> wayroomBoundsList = new List<BoundsInt>();
    public static List<HashSet<Vector2Int>> wayroomCorridorList = new List<HashSet<Vector2Int>>();
    internal static void CreateWayRoomBounds(int dungeonWidth, int dungeonHeight, int wayroomWidth, List<HashSet<int>> wayRoomList, TilemapVisualizer tilemapVisualizer)
    {
        wayroomBoundsList = new List<BoundsInt>();
        int roomCount = 0;
        for (int x = -dungeonWidth; x < 0; x += (wayroomWidth))
        {
            if (roomCount >= wayRoomList.Count) continue;
            for (int y = -dungeonHeight; y < 0; y += (wayroomWidth))
            {
                if (roomCount >= wayRoomList.Count) continue;
                BoundsInt room = new BoundsInt(x, y, 0, wayroomWidth, wayroomWidth, 1);
                wayroomBoundsList.Add(room);
                roomCount++;
            }
        }
    }

    internal static void CreateWayRooms(List<HashSet<Vector2Int>> wayroomLayountList, TilemapVisualizer tilemapVisualizer, List<HashSet<int>> wayRoomConnectionsList)
    {
        string text = "connections: ";
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in wayroomLayountList) floor.UnionWith(room);

        tilemapVisualizer.paintFloorTiles(floor);
        WallGenerator.CreateGatelessWalls(floor, tilemapVisualizer);


        //not instead but just do this after
        //instead of doing painting like this make new functinos to place gates and stuff at specific positions since wayrooms are all identical. or try using at preset tileset adn printint ghtat out
        for (int i = 0; i < wayRoomConnectionsList.Count; i++)
        {
            var center = Vector2Int.RoundToInt((Vector2)wayroomBoundsList[i].center);
            List<int> list = wayRoomConnectionsList[i].ToList();
            int offset = i % 2 == 0 ? 0 : 1;
            text += "\n";
            for (int p = 0; p < wayRoomConnectionsList[i].Count; p++)
            {
                if (p > 3)
                {
                    Debug.Log("OVERNFDKN");
                    break;
                }
                text += "" + list[p] + " ";
                var direction = Direction2D.cardinalDirectionsList[p];

                Vector2Int position = center + (3 * direction);
                position = new Vector2Int(position.x, position.y - offset);
                tilemapVisualizer.paintSingleNumberTile(list[p], position); //paint the correct number on the floor infrom of the gat

                position = center + (5 * direction);
                position = new Vector2Int(position.x, position.y - offset);
                Vector2Int secondOffset = p % 2 == 0 ? Vector2Int.right : Vector2Int.down;

                var portal1 = tilemapVisualizer.PaintSingleGate(position);
                portal1.roomNumber = list[p];
                var portal2 = tilemapVisualizer.PaintSingleGate(position + secondOffset);
                portal2.roomNumber = list[p];
                var portal3 = tilemapVisualizer.PaintSingleGate(position - secondOffset);
                portal3.roomNumber = list[p];

                foreach (var collect in RoomCollection.roomCollectionList)
                {
                    if (collect.roomNumber == list[p])
                    {
                        portal1.setDestination(collect.roomCenter);
                        portal2.setDestination(collect.roomCenter);
                        portal3.setDestination(collect.roomCenter);
                    }
                }


            }
            //creates room gates that link to this wayroom
            WallGenerator.CreateGatesForWayroom(tilemapVisualizer, wayroomCorridorList[i], new Vector2Int(center.x, center.y - offset));




        }
        Debug.Log(text);
    }
    public static void ResetWayRooms()
    {
        wayroomBoundsList = new List<BoundsInt>();
        wayroomCorridorList = new List<HashSet<Vector2Int>>();
    }

}

