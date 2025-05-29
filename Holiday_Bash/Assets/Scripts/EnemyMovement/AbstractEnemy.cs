using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour
{
    public float speed = 0.5f;

    private bool movingRight = true;

    /// <summary>
    /// Number of seconds between attacks
    /// </summary>
    public float fireRate = .5f;

    /// <summary>
    /// Radius enemy senses the player at
    /// </summary>
    public float detectionRadius = 8.5f;

    /// <summary>
    /// Radius enemy will attack the player at
    /// </summary>
    public float shootingRange = 5f;
    public float health = 500f;
    public float bulletSpeed = 4.5f;

    public int damage = 20;
    protected float attackTimer = .5f;

    /// <summary>
    /// Effects shooting accuracy.
    /// Its the amount of variation in shooting vector.
    /// </summary>
    public float fireSpreadDegrees = 5f;
    public ProjectileBehavior projectileItem;
    protected float launchOffset = 0.5f;
    public Player player;

    public Rigidbody2D rb;

    protected Vector2Int moveDirection = Vector2Int.zero;

    protected bool isDead = false;

    public float boomTimer = .3f;

    [SerializeField] protected ParticleSystem deathExposion;

    protected bool isCharging = false, forwardsCharging = false;
    private float coastTimer = 0f, coastTime = 2f;

    protected float chargingRadius;

    protected Animator animator;

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
        if (randomizer < 50 || moveDirection == Vector2Int.zero) //small chance to change directions if its moving
        {
            moveDirection = Direction2D.eightDirectionsList[Random.Range(0, Direction2D.eightDirectionsList.Count)];
            extra = 10;
        }

        randomizer = Random.Range(0, 100);
        randomizer += extra;
        if (randomizer < 50) //50% chance to not move
        {
            return;
        }

        var walkSpeed = 2f + Random.Range(-0.7f, 0.8f);
        rb.linearVelocity = new Vector2(moveDirection.x * walkSpeed, moveDirection.y * walkSpeed);
    }

    /// <summary>
    /// Moves towards the player
    /// </summary>
    protected void targetPlayer()
    {
        if (inShootingRange() && !isCharging)
        {
            drift();
            return;
        }
        Vector3 dir = player.transform.position - transform.position;
        dir = dir.normalized;
        moveDirection = new Vector2Int((int)dir.x, (int)dir.y);

        rb.linearVelocity = new Vector2(dir.x * speed, dir.y * speed);

    }


    protected void updateTimers()
    {
        attackTimer += Time.deltaTime;
        coastTimer += Time.deltaTime;
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
        attackTimer = 0f;
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
        bullet.Initialize(new Vector3(newDirection.x, newDirection.y, 0), damage, bulletSpeed);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    /// <summary>
    /// Rapidly move enemy towards the player.
    /// Homing movement until halfway between charging radius then coasts the rest of the way
    /// </summary>
    protected void Charge() //fix this.
    {
        chargingRadius = 2f;
        Vector3 chargeDirection = player.transform.position - transform.position;
        chargeDirection = chargeDirection.normalized; //gets direction without the magnitude
        float distanceData = Vector3.Distance(transform.position, player.transform.position);
        if (distanceData < chargingRadius && forwardsCharging == false)
        {
            //coastTimer = 0;
            //isCharging = false;
            forwardsCharging = true;
            attackTimer = -fireRate;
            return;
        }
        if (forwardsCharging) chargeDirection = rb.linearVelocity.normalized;
        bulletSpeed = player.moveSpeed * 75;
        var force = new Vector2(chargeDirection.x * bulletSpeed, chargeDirection.y * bulletSpeed);
        rb.AddForce(force, ForceMode2D.Force);
        //rb.linearVelocity = force;
    }

    /// <summary>
    /// Run this in fixedUpdate to have drifting, player targeting, and sprite image flipping
    /// </summary>
    protected void defaultUpdateBehavior()
    {
        if (!playerDetected()) drift();

        if (playerDetected() && (coastTimer > coastTime)) targetPlayer();

        if (moveDirection.x >= 0 && !movingRight || moveDirection.x < 0 && movingRight)
        {
            flip();
        }
    }
    private void flip() {
        movingRight = !movingRight;
        transform.localScale *= -1;
    }
}
