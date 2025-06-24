using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TreasureChest : MonoBehaviour
{
    public RoomCollection room;
    public Sprite closedImage;
    public Sprite openImage;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public HealthOrb healthOrb;
    private float interactRange = 1f;
    public int level;
    [NonSerialized] public Player player;
    [NonSerialized] public bool isOpened = false;
    void Start()
    {
        spriteRenderer.sprite = closedImage;
        isOpened = false;
    }
    void Update()
    {
        if (isOpened == false)
        {
            if (IsInRange())
            {
                if (player.GetInteraction())
                {
                    OpenChest();
                }
            }
        }
    }
    public void OpenChest()
    {
        if (isOpened == true) return;

        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.Play();

        isOpened = true;

        int numberOfOrbs = 7 + Random.Range(0, room.difficulty * 5);
        for (int i = 0; i < numberOfOrbs; i++)
        {
            int randomDegree = Random.Range(0, 360);
            Vector3 newDirection = Quaternion.Euler(0, 0, randomDegree) * Vector3.up;
            HealthOrb orb = Instantiate(healthOrb, transform.position, Quaternion.identity);
            orb.moveDirection = newDirection;
            orb.player = player;
            orb.speed = Random.Range(7.5f, 9);
            orb.room = room;
            orb.spreadTime = Random.Range(0.25f, .6f);
        }

        numberOfOrbs = 5 + Random.Range(0, room.difficulty * 5);
        for (int i = 0; i < numberOfOrbs; i++)
        {
            int randomDegree = Random.Range(0, 360);
            Vector3 newDirection = Quaternion.Euler(0, 0, randomDegree) * Vector3.up;
            HealthOrb orb = Instantiate(healthOrb, transform.position, Quaternion.identity);
            orb.moveDirection = newDirection;
            orb.xpMode = true;
            orb.player = player;
            orb.speed = Random.Range(7.5f, 9);
            orb.room = room;
            orb.spreadTime = Random.Range(0.2f, .5f);
        }

        animator.SetBool("opened", true);
    }
    /// <summary>
    /// Checks if plyer is within range
    /// </summary>
    /// <returns>boolean value</returns>
    protected bool IsInRange()
    {
        bool value = false;
        float distanceData = Vector3.Distance(transform.position, player.transform.position);
        if (distanceData < interactRange)
        {
            value = true;
        }
        return value;
    }
}
