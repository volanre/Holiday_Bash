using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class AbstractEnemy : AbstractCharacter
{
    /// <summary>
    /// The time (in secs) all enemies must wait until they become active;
    /// </summary>
    public static float initalPause = 1f;
    
    
    protected float intialPauseTimer = 0, attackTimer = .5f, boomTimer = .3f;
    protected Vector2 moveDirection;
    protected bool isDead = false, isCharging = false, forwardsCharging = false, initialized = false;
    private bool movingRight = true;
    [NonSerialized] public RoomCollection room;


    [Header("Enemy Properties")]
    public ProjectileBehavior projectileItem;
    [SerializeField] protected Rigidbody2D rb;

    /// <summary>
    /// the number of tiles it takes up, its always a square
    /// </summary>
    public int size = 1;
    public float walkSpeed = 1.5f;
    /// <summary>
    /// Radius enemy senses the player at
    /// </summary>
    public float detectionRadius = 8.5f;
    /// <summary>
    /// Radius enemy will attack the player at
    /// </summary>
    public float shootingRange = 5f;
    /// <summary>
    /// Effects shooting accuracy.
    /// Its the amount of variation in shooting vector.
    /// </summary>
    public float fireSpreadDegrees = 5f;
    [SerializeField] protected ParticleSystem deathExposion;
    public Player player;

    /*  METHODS    */


    /// <summary>
    /// Checks if plyer is within detection range
    /// </summary>
    /// <returns>boolean value</returns>
    protected bool playerDetected()
    {
        bool value = false;
        float distanceData = Vector3.Distance(transform.position, player.transform.position);
        if (distanceData < detectionRadius)
        {
            value = true;
        }
        return value;
    }

    /// <summary>
    /// Checks if plyer is within shooting range
    /// </summary>
    /// <returns>boolean value</returns>
    protected bool inShootingRange()
    {
        bool value = false;
        float distanceData = Vector3.Distance(transform.position, player.transform.position);
        if (distanceData < shootingRange)
        {
            value = true;
        }
        return value;
    }

    /// <summary>
    /// Moves the rigidbody of the enemy in a random direction.
    /// </summary>
    protected void drift()
    {
        var randomizer = Random.Range(0, 1000);
        int extra = 0;
        if (randomizer < 50) //small chance to change directions if its moving
        {
            moveDirection = Direction2D.eightDirectionsList[Random.Range(0, Direction2D.eightDirectionsList.Count)];
            extra = 10;
        }

        randomizer = Random.Range(0, 1000);
        randomizer += extra;
        if (randomizer < 25) //small chance to not move
        {
            moveDirection = Vector2.zero;
        }

        if (!room.roomFloor.Contains(new Vector2Int((int)transform.position.x, (int)transform.position.y)))
        {
            moveDirection = room.roomCenter - new Vector2Int((int)transform.position.x, (int)transform.position.y);
            moveDirection = moveDirection.normalized;
        }

        var realWalkSpeed = walkSpeed + Random.Range(-0.7f, 0.8f);
        rb.linearVelocity = new Vector2(moveDirection.x * realWalkSpeed, moveDirection.y * realWalkSpeed);
    }
    /*
    
    /// <summary>
    /// Moves towards the player
    /// </summary>
    protected void targetPlayer()
    {
        if (isCharging) return;
        if (inShootingRange() && !isCharging)
        {
            drift();
            return;
        }
        //Vector2Int currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Vector2 bottomLeft = gameObject.GetComponent<Collider2D>().bounds.min;
        Vector2Int currentPosition = Vector2Int.FloorToInt(bottomLeft);
        //player.playerFloodField[currentPosition];
        Vector2Int targetPosition = Vector2Int.zero;

        float bestDot = float.NegativeInfinity;
        Vector2 bestDir = Vector2.zero;
        bool betterOneExists = false;
        foreach (var direction in Direction2D.cardinalDirectionsList)
        {
            Vector2Int offset = direction;
            if (direction == Vector2Int.up || direction == Vector2Int.right)
            {
                offset = direction * size;
            }
            Vector2Int nextPos = currentPosition + offset;

            if (!room.roomFloor.Contains(nextPos)) continue;
            if (player.playerFloodField[nextPos] < player.playerFloodField[currentPosition])
            {
                betterOneExists = true;
                targetPosition = nextPos;
            }
            else if (player.playerFloodField[nextPos] == player.playerFloodField[currentPosition])
            {
                if (!betterOneExists)
                {
                    Vector2 dirVector = direction;
                    float dot = Vector2.Dot(dirVector.normalized, rb.linearVelocity.normalized);
                    if (dot > bestDot)
                    {
                        bestDot = dot;
                        bestDir = dirVector;
                    }
                }
            }
        }
        if (!betterOneExists) //in case all neighboring costs are equal
        {
            moveDirection = bestDir.normalized;
        }
        else
        {
            moveDirection = targetPosition - currentPosition;
            moveDirection = moveDirection.normalized;
        }

        // Vector2 dir = currentPosition - targetPosition;
        // dir = dir.normalized;


        //rb.linearVelocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
        rb.linearVelocity = moveDirection * speed;
    }

    */
    protected void targetPlayer()
    {
        if (isCharging) return;
        if (inShootingRange() && !isCharging)
        {
            drift();
            return;
        }
        Vector2 bottomLeft = gameObject.GetComponent<Collider2D>().bounds.min;
        Vector2Int currentPosition = Vector2Int.FloorToInt(bottomLeft);

        moveDirection = GradientDescent(currentPosition);
        
        //rb.linearVelocity = moveDirection * speed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, moveDirection * speed, 0.8f);
        
    }

    /// <summary>
    /// Gets the normalized vector of the lowest cost direction in the players FloodField
    /// </summary>
    /// <param name="currentPosition"></param>
    /// <returns></returns>
    Vector2 GradientDescent(Vector2Int currentPosition)
    {
        List<float> costValues = new List<float>();
        foreach (var dir in Direction2D.cardinalDirectionsList)
        {
            Vector2Int newPos = currentPosition + dir;
            if (player.playerFloodField.ContainsKey(newPos) && player.playerClearenceMap[newPos] >= size)
            {
                costValues.Add(player.playerFloodField[newPos]);
            }
            else {
                costValues.Add(9999);
            }
        }

        // Compute gradient (negative for descent)
        //Vector2 gradient = new Vector2(left - right, down - up);
        Vector2 gradient = new Vector2(costValues[3] - costValues[1], costValues[2] - costValues[0]);

        return gradient.normalized;
    }


    protected void updateTimers()
    {
        attackTimer += Time.deltaTime;
        intialPauseTimer += Time.deltaTime;
    }

    protected void checkIfDead()
    {
        if (isDead) return;
        if (health <= 0 && !isDead)
        {

            gameObject.GetComponent<Collider2D>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

            deathExposion.Play();
            isDead = true;
            // Bryce Made this :)
        }
    }
    protected void suicide()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Instantiates and shoots a projectile towards the player.
    /// Takes into account shooting accuracy.
    /// </summary>
    protected void Shoot()
    {
        float maxAngleRadians = fireSpreadDegrees * Mathf.Deg2Rad;
        Vector3 shootDirection = player.transform.position - transform.position;
        shootDirection = shootDirection.normalized; //gets direction without the magnitude

        var upDown = Random.Range(0, 1) == 0 ? Vector3.up : Vector3.down;

        Vector3 newDirection = Vector3.RotateTowards(shootDirection, Vector3.Cross(shootDirection, upDown), maxAngleRadians, 0f);
        newDirection = newDirection.normalized;


        Vector3 center = GetComponent<BoxCollider2D>().bounds.center;
        //Debug.Log("center: " + center.x + ", " + center.y);
        //Debug.Log("shootvector: " + newDirection.x + ", " + center.y);
        Vector3 bulletPosition = new Vector3(center.x + launchOffset * newDirection.x, center.y + launchOffset * newDirection.y, 0);

        var bullet = Instantiate(projectileItem, bulletPosition, transform.rotation);
        bullet.targetPlayer = true;
        bullet.targetEnemy = false;
        bullet.Initialize(new Vector3(newDirection.x, newDirection.y, 0), CalculateEffectiveDamage(attack), bulletSpeed);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    /// <summary>
    /// Rapidly move enemy towards the player.
    /// Homing movement until halfway between charging radius then coasts the rest of the way
    /// </summary>
    protected void Charge() //fix this.
    {
        var chargingRadius = 2f;
        Vector3 chargeDirection = player.transform.position - transform.position;
        chargeDirection = chargeDirection.normalized; //gets direction without the magnitude
        float distanceData = Vector3.Distance(transform.position, player.transform.position);
        if (distanceData < chargingRadius && forwardsCharging == false)
        {
            //coastTimer = 0;
            //isCharging = false;
            forwardsCharging = true;
            attackTimer = -fireRate;
            moveDirection = new Vector2(chargeDirection.x, chargeDirection.y);
            return;
        }
        if (!forwardsCharging) moveDirection = new Vector2(chargeDirection.x, chargeDirection.y);
        var force = new Vector2(moveDirection.x * bulletSpeed, moveDirection.y * bulletSpeed);
        //rb.AddForce(force, ForceMode2D.Force);
        rb.linearVelocity = force;
    }

    /// <summary>
    /// Run this in fixedUpdate to have drifting, player targeting, and sprite image flipping
    /// </summary>
    protected void defaultUpdateBehavior()
    {

        if (!playerDetected()) drift();

        if (playerDetected()) targetPlayer();

        if (moveDirection.x >= 0 && !movingRight || moveDirection.x < 0 && movingRight)
        {
            flip();
        }
        
    }

    protected bool checkInitialized()
    {

        if (intialPauseTimer < initalPause)
        {
            initialized = false;
            gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        }
        else
        {
            if (!initialized)
            {
                moveDirection = Direction2D.getRandomCardinalDirection();
                health = maxHealth;
                initialized = true;
                gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }


        return initialized;
    }
    private void flip()
    {
        movingRight = !movingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    // public void TakeDamage(int damageTaken)
    // {
    //     health -= damageTaken;
    //     spriteRenderer.color = Color.red;
    //     Invoke("ResetColor", 0.07f);
    // }
    // private void ResetColor()
    // {
    //     spriteRenderer.color = Color.white;
    // }
}
