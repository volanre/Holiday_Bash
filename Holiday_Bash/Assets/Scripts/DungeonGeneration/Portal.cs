using System;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private static List<GameObject> masterPortalList;

    
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();
    private Vector2Int destination = Vector2Int.zero;

    [NonSerialized]
    public int roomNumber;

    public static bool isOn = false;


    void Start()
    {
        masterPortalList.Add(gameObject);
    }
    // public static void setRoomDesinations()
    // {
    //     foreach (var portal in masterPortalList)
    //     {
    //         for(var room)
    //         Vector2Int destination = RoomCollection.roomCollectionList
    //     }
    // }
    public static void toggleActive(bool side)
    {
        isOn = side;

        // foreach (var portal in masterPortalList)
        // {
        //     portal.GetComponent<SpriteRenderer>().color = isOn == true ? Color.white : Color.gray;
        // }
    }

    public void setDestination(Vector2Int place)
    {
        destination = place;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            return;
        }
        if (portalObjects.Contains(collision.gameObject))
        {
            return;
        }
        
        // if (destination.TryGetComponent(out Portal destinationPortal))
        // {
        //     destinationPortal.portalObjects.Add(collision.gameObject);
        // }
        if (isOn) collision.transform.position = new Vector3(destination.x, destination.y, 0);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        portalObjects.Remove(collision.gameObject);
    }

    private static void suicide(GameObject thing)
    {
        Destroy(thing);
    }


    public static void resetPortalList()
    {
        // foreach (var portal in masterPortalList)
        // {
        //     suicide(portal);
        // }
        masterPortalList = new List<GameObject>();
    }
    


}
