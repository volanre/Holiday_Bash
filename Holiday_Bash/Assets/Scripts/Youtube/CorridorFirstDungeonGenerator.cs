using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int corridorLength = 14;

    [SerializeField]
    private int corridorCount = 5;

    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 0.8f;

    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridorList = CreateCorridors(floorPositions, potentialRoomPositions);

        

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomsAtDeadEnds(deadEnds, roomPositions);
        
        for (int i = 0; i < corridorList.Count; i++)
        {
            corridorList[i] = IncreaseCorridorSize(corridorList[i]);
            floorPositions.UnionWith(corridorList[i]);
        }

        floorPositions.UnionWith(roomPositions);


        // HashSet<Vector2Int> startingRoom = new HashSet<Vector2Int>
        // {
        //     Vector2Int.zero
        // };

        // floorPositions.UnionWith(startingRoom);

        tilemapVisualizer.paintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private List<Vector2Int> IncreaseCorridorSize(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        if (corridor.Contains(corridor[0] + Vector2Int.up) || corridor.Contains(corridor[0] + Vector2Int.down))
        {
            foreach (var position in corridor)
            {
                newCorridor.Add(position + Vector2Int.left);
                newCorridor.Add(position + Vector2Int.right);
            }
        }
        else
        {
            foreach (var position in corridor)
            {
                newCorridor.Add(position + Vector2Int.up);
                newCorridor.Add(position + Vector2Int.down);
            }
        }

        return newCorridor;
    }

    private void CreateRoomsAtDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomPos)
    {

        foreach (var position in deadEnds)
        {
            if (roomPos.Contains(position) == false)
            {
                var roomFLoor = RunRandomWalk(randomWalkParameters, position);
                roomPos.UnionWith(roomFLoor);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            int neighborsCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                {
                    neighborsCount++;
                }
            }
            if (neighborsCount == 1)
            {
                deadEnds.Add(position);
            }
        }
        if (deadEnds.Contains(Vector2Int.zero) == false)
        {
            deadEnds.Add(Vector2Int.zero);
        }

        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);
        List<Vector2Int> roomToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPos in roomToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPos);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(startPosition);
        List<List<Vector2Int>> corridorList = new List<List<Vector2Int>>();

        for (int i = 0; i < corridorCount; i++)
        {
            var path = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = path[path.Count - 1];
            corridorList.Add(path);
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(path);
        }

        return corridorList;
       
    }
}
