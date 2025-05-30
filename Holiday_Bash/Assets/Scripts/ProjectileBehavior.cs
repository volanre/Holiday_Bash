using System;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    private float speed = 7.5f;

    public float extraRotationDegrees = 0;
    private Vector3 direction = Vector3.zero;

    public int damage = 100;

    public AudioClip playerImpactNoise;

    [NonSerialized]
    public bool targetEnemy = true, targetPlayer = true;
    [NonSerialized] public bool isTangible;

    //private Player player;

    Vector2 Rotate(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    void Start()
    {
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
        var velocity = gameObject.GetComponent<Rigidbody2D>().linearVelocity.normalized;
        gameObject.transform.right = Rotate(velocity, extraRotationDegrees);
        gameObject.GetComponent<Collider2D>().isTrigger = true;
    }

    void Update()
    {
        // transform.position += direction * Time.deltaTime * speed;
        //gameObject.GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
    }

    // private void OnCollisionEnter2D(Collision2D collision) //Runs if its a solid projectile
    // {
    //     GameObject other = collision.gameObject;
    //     Destroy(gameObject);
    //     if (other.CompareTag("Enemy") && targetEnemy)
    //     {
    //         other.GetComponent<AbstractEnemy>().TakeDamage(damage);
    //     }
    //     else if (other.CompareTag("Player") && targetPlayer)
    //     {
    //         Player player = other.GetComponent<Player>();
    //         if (playerImpactNoise != null)
    //         {
    //             player.TakeDamage(damage, playerImpactNoise);
    //         }
    //         else
    //         {
    //             player.TakeDamage(damage);
    //         }
    //     }
    // }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;

        if (gameObject.CompareTag("Enemy_Bullet"))
        {
            if (other.CompareTag("Enemy_Bullet")) return;
            else if (other.CompareTag("Player_Bullet"))
            {
                if (!isTangible)
                {
                    return;
                }
            }
            else if (other.CompareTag("Prop"))
            {
                if (!isTangible)
                {
                    return;
                }
            }
        }
        else if (gameObject.CompareTag("Player_Bullet"))
        {
            if (other.CompareTag("Player_Bullet")) return;
            else if (other.CompareTag("Enemy_Bullet"))
            {
                if (!other.GetComponent<ProjectileBehavior>().isTangible)
                {
                    return;
                }
            }
        }

        Destroy(gameObject);

        if (other.CompareTag("Enemy") && targetEnemy)
        {
            other.GetComponent<AbstractEnemy>().TakeDamage(damage);
        }
        else if (other.CompareTag("Player") && targetPlayer)
        {
            Player player = other.GetComponent<Player>();
            if (playerImpactNoise != null)
            {
                player.TakeDamage(damage, playerImpactNoise);
            }
            else
            {
                player.TakeDamage(damage);
            }
        }
    }

    public void Initialize(Vector3 direction, int damage = 100, float speed = 7.5f, bool tangible = true)
    {
        this.direction = direction;
        this.damage = damage;
        this.speed = speed;
        this.isTangible = tangible;
    }
    
        
    
}
