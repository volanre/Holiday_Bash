using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour
{
    public float speed = 0.5f;
    public float fireRate = .5f;
    public float detectionRadius = 8.5f;

    public float shootingRange = 5f;
    public float health = 500f;
    public float bulletSpeed = 4.5f;

    public int damage = 20;
    protected float attackTimer = .5f;

    public float fireSpreadDegrees = 5f;
    public ProjectileBehavior projectileItem;
    protected float launchOffset = 0.5f;
    public Player player;

    public Rigidbody2D rb;

    private Vector2Int moveDirection = Vector2Int.zero;

    protected bool isDead = false;

    [SerializeField] protected ParticleSystem deathExposion;

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
    /// Moves the rigidbody of the enemy based on its speed and random variables.
    /// If player is within range it the function doesn't run.
    /// </summary>
    protected void drift()
    {
        if (playerDetected()) return;
        var randomizer = Random.Range(0, 1000);
        int extra = 0;
        if (randomizer < 50) //small chance to change directions if its moving
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

        var walkSpeed = 1f + Random.Range(-0.7f, 0.8f);
        rb.linearVelocity = new Vector2(moveDirection.x * walkSpeed, moveDirection.y * walkSpeed);
    }

    /// <summary>
    /// Moves towards the player if player is detected and not inside shooting range.
    /// </summary>
    protected void targetingPlayer()
    {
        if (!playerDetected()) return;
        if (inShootingRange()) return;

        var dir = player.transform.position - transform.position;
        moveDirection = new Vector2Int((int)dir.x, (int)dir.y);

        rb.linearVelocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);

    }


    protected void updateTimers()
    {
        attackTimer += Time.deltaTime;
    }

    protected void checkIfDead()
    {
        if (isDead) return;
        if (health <= 0 && !isDead)
        {
            isDead = true;
            gameObject.GetComponent<Collider2D>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

            deathExposion.Play();
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
        bullet.Initialize(new Vector3(newDirection.x, newDirection.y, 0), damage, bulletSpeed);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }
}
