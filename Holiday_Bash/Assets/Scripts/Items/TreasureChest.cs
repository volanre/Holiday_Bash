using System;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public RoomCollection room;
    public Sprite closedImage;
    public Sprite openImage;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private float interactRange = 1.5f;
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
                Debug.Log("interactions: " + player.GetInteraction());
                if (player.GetInteraction())
                {
                    Debug.Log("button pressed!!!");
                    OpenChest();
                }
            }
        }
    }
    public void OpenChest()
    {
        isOpened = true;
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
