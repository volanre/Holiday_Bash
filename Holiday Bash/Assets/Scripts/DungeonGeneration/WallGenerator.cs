using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class WallGenerator
{
    public static HashSet<Vector2Int> allWallPositions = new HashSet<Vector2Int>();

    public static void resetWallPositions()
    {
        allWallPositions = new HashSet<Vector2Int>();
    }

    public static void CreateGatelessWalls(HashSet<Vector2Int> floorPosiitons, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = FindWallsInDirections(floorPosiitons, Direction2D.cardinalDirectionsList);
        var cornerWallPositions = FindWallsInDirections(floorPosiitons, Direction2D.diagonalDirectionsList);

        CreateBasicWalls(tilemapVisualizer, basicWallPositions, floorPosiitons);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPosiitons);

    }

    public static void CreateWalls(HashSet<Vector2Int> floorPosiitons, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = FindWallsInDirections(floorPosiitons, Direction2D.cardinalDirectionsList);
        var cornerWallPositions = FindWallsInDirections(floorPosiitons, Direction2D.diagonalDirectionsList);

        CreateBasicWalls(tilemapVisualizer, basicWallPositions, floorPosiitons);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPosiitons);

        allWallPositions.UnionWith(basicWallPositions);
        allWallPositions.UnionWith(cornerWallPositions);
        //CreateGates(tilemapVisualizer, allWallPositions, floorPosiitons);

    }

    public static void CreateCornerWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> wallPositions, HashSet<Vector2Int> floorPosiitons)
    {
        foreach (var position in wallPositions)
        {
            string neighborBinary = "";
            foreach (var direction in Direction2D.eightDirectionsList)
            {
                var neighborPosition = position + direction;
                if (floorPosiitons.Contains(neighborPosition)) neighborBinary += "1";
                else neighborBinary += "0";
            }
            tilemapVisualizer.paintSingleCornerWall(position, neighborBinary);
        }
        
    }

    public static void CreateGatesForWayroom(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> floorPositions, Vector2Int wayroomLocation)
    {
        foreach (var position in allWallPositions)
        {
            if (floorPositions.Contains(position))
            {
                var portal = tilemapVisualizer.PaintSingleGate(position);
                //tilemapVisualizer.PaintSingleGate(position);
                bool value = false;
                foreach (var roomCollection in RoomCollection.roomCollectionList)
                {
                    if (roomCollection.roomBound.Contains((Vector3Int)position))
                    {
                        portal.roomNumber = roomCollection.roomNumber;
                        portal.setDestination(wayroomLocation);
                        value = true;
                        //Debug.Log(portal.roomNumber);
                    }
                }
                if (value == false)
                {
                    Debug.Log("portalnumber failed");
                }
                
                
            }
        }
    }

    

    public static void CreateBasicWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPositions)
        {
            string neightborsBinary = "";
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                var neighborPosition = position + direction;
                if (floorPositions.Contains(neighborPosition))
                {
                    neightborsBinary += "1";
                }
                else neightborsBinary += "0";
            }
            tilemapVisualizer.PaintSingleBasicWall(position, neightborsBinary);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPosiitons, List<Vector2Int> directionsList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPosiitons)
        {
            foreach (var direction in directionsList)
            {
                var neighborPosition = position + direction;
                if (floorPosiitons.Contains(neighborPosition) == false)
                {
                    wallPositions.Add(neighborPosition);
                }
            }
        }

        return wallPositions;
    }
    
}
