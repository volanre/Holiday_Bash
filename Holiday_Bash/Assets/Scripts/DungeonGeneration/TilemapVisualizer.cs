using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    public Tilemap floorTilemap, wallTilemap;


    [SerializeField]
    private Portal portal;

    [SerializeField]
    private TileBase floorTile, gateTile,
    wallTop, wallSideRight, wallSideLeft, wallBot, wallFull,
    wallInnerCornerDownLeft, wallInnerCornerDownRight,
    wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

    [SerializeField]
    private List<TileBase> floorList,numberFloorList;

    public void paintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorList);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, List<TileBase> tileList)
    {
        foreach (var position in positions)
        {
            TileBase tile = tileList[Random.Range(0, tileList.Count)];
            paintSingleTile(tilemap, tile, position);
        }
    }

    private void paintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }


    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypesHelper.wallTop.Contains(typeAsInt)) tile = wallTop;
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt)) tile = wallSideRight;
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt)) tile = wallSideLeft;
        else if (WallTypesHelper.wallBottom.Contains(typeAsInt)) tile = wallBot;
        else if (WallTypesHelper.wallFull.Contains(typeAsInt)) tile = wallFull;

        if (tile != null) paintSingleTile(wallTilemap, tile, position);


    }

    internal void paintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt)) tile = wallInnerCornerDownLeft;
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt)) tile = wallInnerCornerDownRight;
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt)) tile = wallDiagonalCornerDownLeft;
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt)) tile = wallDiagonalCornerDownRight;
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt)) tile = wallDiagonalCornerUpLeft;
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt)) tile = wallDiagonalCornerUpRight;
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt)) tile = wallFull;
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt)) tile = wallBot;



        if (tile != null) paintSingleTile(wallTilemap, tile, position);
    }

    internal Portal PaintSingleGate(Vector2Int position)
    {
        paintSingleTile(wallTilemap, gateTile, position);
        var thisportal = Instantiate(portal, wallTilemap.GetCellCenterWorld((Vector3Int)position), Quaternion.identity);
        return thisportal;
    }

    internal void paintSingleNumberTile(int connectionNumber, Vector2Int position)
    {
        if (connectionNumber >= numberFloorList.Count) connectionNumber = numberFloorList.Count - 1;
        paintSingleTile(floorTilemap, numberFloorList[connectionNumber], position);
    }
}
