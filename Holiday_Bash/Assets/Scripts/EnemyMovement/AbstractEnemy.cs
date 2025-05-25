using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour
{
    public float speed = 3f;
    public float fireRate = .5f;
    public float detectionRadius = 8.5f;
    public float health = 500f;
    public float bulletSpeed = 4.5f;
    protected float attackTimer = .5f;
    public ProjectileBehavior projectileItem;
    protected float launchOffset = 0.5f;
    public Player player;

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
    protected void updateTimers()
    {
        attackTimer += Time.deltaTime;
    }

    protected void Shoot()
    {
        
        attackTimer = 0f;
        Vector3 shootDirection = player.transform.position - transform.position;
        shootDirection = shootDirection.normalized; //gets direction without the magnitude

        Vector3 center = GetComponent<BoxCollider2D>().bounds.center;
        Debug.Log("center: " + center.x + ", " + center.y);
        Debug.Log("shootvector: " + shootDirection.x + ", " + center.y);
        Vector3 bulletPosition = new Vector3(center.x + launchOffset * shootDirection.x - .4f, center.y + launchOffset * shootDirection.y - .5f, 0);

        var bullet = Instantiate(projectileItem, bulletPosition, transform.rotation);
        bullet.direction = new Vector3(shootDirection.x, shootDirection.y, 0);
        bullet.speed = bulletSpeed;

        
    }
}
