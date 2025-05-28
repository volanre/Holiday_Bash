using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using System.Linq;
using Random = UnityEngine.Random;



public class PropPlacementManager : MonoBehaviour
{

    [SerializeField]
    private List<Prop> propList;

    [SerializeField, Range(0, 1)]
    private float cornerPlaceChance = 0.7f;

    [SerializeField]
    private GameObject propPrefab;

    public void ProcessRooms()
    {
        if (RoomCollection.roomCollectionList == null) return;
        foreach (var room in RoomCollection.roomCollectionList)
        {
            //Corner
            List<Prop> cornerProps = propList.Where(x => x.corner).ToList();
            placeCornerProps(room, cornerProps);

            //Left
            List<Prop> leftWallProps = propList
            .Where(x => x.nearLeftWall)
            .OrderByDescending(x => x.propSize.x * x.propSize.y)
            .ToList();

            placeProps(room, leftWallProps, room.roomData.floorNearLeftWall, PlacementOrigin.BottomLeft);

            //Right
            List<Prop> rightWallProps = propList
            .Where(x => x.nearRightWall)
            .OrderByDescending(x => x.propSize.x * x.propSize.y)
            .ToList();

            placeProps(room, rightWallProps, room.roomData.floorNearRightWall, PlacementOrigin.TopRight);

            //Up
            List<Prop> upWallProps = propList
            .Where(x => x.nearUpWall)
            .OrderByDescending(x => x.propSize.x * x.propSize.y)
            .ToList();

            placeProps(room, upWallProps, room.roomData.floorNearUpWall, PlacementOrigin.TopLeft);

            //Down
            List<Prop> downWallProps = propList
            .Where(x => x.nearDownWall)
            .OrderByDescending(x => x.propSize.x * x.propSize.y)
            .ToList();

            placeProps(room, downWallProps, room.roomData.floorNearDownWall, PlacementOrigin.BottomRight);
            
            //Middle
            List<Prop> middleProps = propList
            .Where(x => x.innerMiddle)
            .OrderByDescending(x => x.propSize.x * x.propSize.y)
            .ToList();
            
            placeProps(room, middleProps, room.roomData.floorInsideMiddle, PlacementOrigin.BottomRight);
        }
    }

    private void placeProps(RoomCollection room, List<Prop> wallProps, HashSet<Vector2Int> availableTiles, PlacementOrigin placement)
    {
        HashSet<Vector2Int> tempPositions = new HashSet<Vector2Int>(availableTiles);
        tempPositions.ExceptWith(room.roomData.floorInPath);
        foreach (var futureProp in wallProps)
        {
            if (Random.value < 0.2f) continue; //20% chance to not place this prop

            int quantity = Random.Range(futureProp.placementQuantityMin, futureProp.placementQuantityMax + 1);
            for (int i = 0; i < quantity; i++)
            {
                tempPositions.ExceptWith(room.propPositions);

                List<Vector2Int> positioins = tempPositions.OrderBy(thingy => Guid.NewGuid()).ToList();

                if (!TryBrutePlacement(room, futureProp, positioins, placement)) break;
            }
        }
    }

    private bool TryBrutePlacement(RoomCollection room, Prop futureProp, List<Vector2Int> positioins, PlacementOrigin placement)
    {
        for (int i = 0; i < positioins.Count; i++)
        {
            Vector2Int position = positioins[i];
            if (room.propPositions.Contains(position)) continue;

            List<Vector2Int> freeSpace = TryFittingProp(futureProp, positioins, position, placement);

            if (freeSpace.Count == futureProp.propSize.x * futureProp.propSize.y)
            {
                placePropAt(room, position, futureProp);

                foreach (var posi in freeSpace) room.propPositions.Add(posi);

                if (futureProp.placeAsGroup) placeGroupPropAt(room, position, futureProp, 1);

                return true;
            }
        }
        return false;
    }

    private List<Vector2Int> TryFittingProp(Prop futureProp, List<Vector2Int> positioins, Vector2Int origin, PlacementOrigin placement)
    {
        List<Vector2Int> freeSpots = new();

        if (placement == PlacementOrigin.BottomLeft)
        {
            for (int x = 0; x < futureProp.propSize.x; x++)
            {
                for (int y = 0; y < futureProp.propSize.y; y++)
                {
                    Vector2Int temoPos = origin + new Vector2Int(x, y);
                    if (positioins.Contains(temoPos)) freeSpots.Add(temoPos);
                }
            }
        }
        else if (placement == PlacementOrigin.BottomRight)
        {
            for (int x = -futureProp.propSize.x + 1; x <= 0; x++)
            {
                for (int y = 0; y < futureProp.propSize.y; y++)
                {
                    Vector2Int temoPos = origin + new Vector2Int(x, y);
                    if (positioins.Contains(temoPos)) freeSpots.Add(temoPos);
                }
            }
        }
        else if (placement == PlacementOrigin.TopLeft)
        {
            for (int x = 0; x < futureProp.propSize.x; x++)
            {
                for (int y = -futureProp.propSize.y + 1; y <= 0; y++)
                {
                    Vector2Int temoPos = origin + new Vector2Int(x, y);
                    if (positioins.Contains(temoPos)) freeSpots.Add(temoPos);
                }
            }
        }
        else //Top Right
        {
            for (int x = -futureProp.propSize.x + 1; x <= 0; x++)
            {
                for (int y = -futureProp.propSize.y + 1; y <= 0; y++)
                {
                    Vector2Int temoPos = origin + new Vector2Int(x, y);
                    if (positioins.Contains(temoPos)) freeSpots.Add(temoPos);
                }
            }
        }




        return freeSpots;
    }

    private void placeCornerProps(RoomCollection room, List<Prop> cornerProps)
    {
        if (cornerProps.Count <= 0) return;
        foreach (var cornerTile in room.roomData.floorNearCorner)
        {
            if (Random.value < cornerPlaceChance)
            {
                Prop futureProp = cornerProps[Random.Range(0, cornerProps.Count)];
                placePropAt(room, cornerTile, futureProp);
                if (futureProp.placeAsGroup)
                {
                    placeGroupPropAt(room, cornerTile, futureProp, 2);
                }
            }
            else
            {
                cornerPlaceChance = Mathf.Clamp01(cornerPlaceChance + 0.1f);
            }
        }
    }

    private GameObject placePropAt(RoomCollection room, Vector2Int position, Prop futureProp)
    {
        GameObject prop = Instantiate(propPrefab);
        SpriteRenderer spriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();

        spriteRenderer.sprite = futureProp.propSprite;

        CapsuleCollider2D collider2D = spriteRenderer.gameObject.AddComponent<CapsuleCollider2D>();
        collider2D.offset = Vector2.zero;
        if (futureProp.propSize.x > futureProp.propSize.y)
        {
            collider2D.direction = CapsuleDirection2D.Horizontal;
        }

        Vector2 size = new Vector2(futureProp.propSize.x * 0.8f, futureProp.propSize.y * 0.8f);
        collider2D.size = size;

        prop.transform.localPosition = (Vector2)position;

        spriteRenderer.transform.localPosition = (Vector2)futureProp.propSize * 0.5f;

        room.propPositions.Add(position);
        room.propReferences.Add(prop);
        return prop;
    }

    private void placeGroupPropAt(RoomCollection room, Vector2Int position, Prop futureProp, int searchOffset)
    {
        int count = Random.Range(futureProp.groupQuantityMin, futureProp.groupQuantityMax);
        count = Mathf.Clamp(count, 0, 8);

        //find empty spots
        List<Vector2Int> emptySpots = new List<Vector2Int>();
        for (int x = -searchOffset; x <= searchOffset; x++)
        {
            for (int y = -searchOffset; y < searchOffset; y++)
            {
                Vector2Int pos = position + new Vector2Int(x, y);
                if (room.roomFloor.Contains(pos) && !room.roomData.floorInPath.Contains(pos) && !room.propPositions.Contains(pos))
                {
                    emptySpots.Add(pos);
                }
            }
        }


        emptySpots.OrderBy(x => Guid.NewGuid());

        //place the props
        count = count < emptySpots.Count ? count : emptySpots.Count;
        for (int i = 0; i < count; i++)
        {
            placePropAt(room, emptySpots[i], futureProp);
        }
    }
}

public enum PlacementOrigin {
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}
