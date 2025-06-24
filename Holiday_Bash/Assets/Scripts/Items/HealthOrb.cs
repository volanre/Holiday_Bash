using System;
using System.Linq;
using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    //its called a health orb but it also funcitons as an xp orb
    public RoomCollection room;
    private float detectionRange = 5f;
    public Vector2 moveDirection = Vector2.zero;
    [NonSerialized] public Player player;
    private Rigidbody2D rb;
    private float spreadTimer = 0f;
    public float spreadTime = 1f;
    public float speed = 1f;
    [NonSerialized] public bool xpMode = false;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (xpMode)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 1, 0);
        }
        if (!room.roomFloor.Contains(new Vector2Int((int)transform.position.x, (int)transform.position.y)))
        {
            Destroy(gameObject);
            return;
        }
        if (spreadTimer < spreadTime)
        {
            spreadTimer += Time.deltaTime;
            return;
        }

        if (IsInRange(detectionRange))
        {
            var dir = player.transform.position - transform.position;
            moveDirection = dir.normalized;
        }
        else
        {
            moveDirection = Vector2.zero;
        }
    }
    void FixedUpdate()
    {
        if (DistanceToPlayer() == 0) return;
        if (spreadTimer > spreadTime)
        {
            speed = 1.3f / DistanceToPlayer();
        }
        rb.linearVelocity = speed * moveDirection;
    }
    protected bool IsInRange(float range)
    {
        if (DistanceToPlayer() < range)
        {
            return true;
        }
        return false;
    }
    protected float DistanceToPlayer() {
        return Vector3.Distance(transform.position, player.transform.position);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            if (!xpMode)
            {
                int healingAmount = (int)(player.maxHealth * 0.015);
                if (healingAmount <= 0) healingAmount = 1;
                player.Heal(healingAmount);
            }
            else
            {
                player.GainXP(UnityEngine.Random.Range(10,17));
            }
        }
        

    }
}
