using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private int FloorOfTheDungeon = 1;
    [Header("Resources")]
    [SerializeField] private Player playerPrefab;

    [Header("References")]
    [SerializeField] private RoomFirstDungeonGenerator dungeon;

    [SerializeField] private PropPlacementManager propPlacer;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject deathCanvasScreen;
    [SerializeField] private GameObject settingsCanvasScreen;
    [SerializeField] private GameObject gameCanvasScreen;
    [SerializeField] HealthBarUI HP_Bar_UI;

    [Header("Settings")]
    [SerializeField, Range(0, 10)] public static float SFXVolume = 1;
    [SerializeField, Range(0, 10)] public static float MusicVolume = 0.7f;

    private Player player;

    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        
        FloorOfTheDungeon = 1;
        player = Instantiate(playerPrefab);

        player.healthBar = HP_Bar_UI;
        mainCamera.transform.SetParent(player.transform, false);
        enemyManager.player = player;

        deathCanvasScreen.SetActive(false);
        settingsCanvasScreen.SetActive(false);
        gameCanvasScreen.SetActive(true);
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
        if (Player.isAlive)
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
        else
        {
            deathCanvasScreen.SetActive(true);
            gameCanvasScreen.SetActive(false);
            settingsCanvasScreen.SetActive(false);
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

    public void LoadScene(int index)
    {
        ScreenManager.LoadScene(index);
    }
    public void restartGame()
    {
        player = null;
        mainCamera.transform.parent = null;
        mainCamera.transform.SetSiblingIndex(0);
        GameObject[] clones = GameObject.FindGameObjectsWithTag("Enemy");
        clones.Union(GameObject.FindGameObjectsWithTag("Player"));
        clones.Union(GameObject.FindGameObjectsWithTag("Enemy_Bullet"));
        clones.Union(GameObject.FindGameObjectsWithTag("Player_Bullet"));

        foreach (GameObject clone in clones)
        {
            Destroy(clone);
        }

        ScreenManager.ResetCurrentScene();
        StartGame();
    }

}
