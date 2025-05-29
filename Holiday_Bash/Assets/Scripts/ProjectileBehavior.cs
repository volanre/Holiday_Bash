using System;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    private float speed = 7.5f;

    public float extraRotationDegrees = 0;
    private Vector3 direction = Vector3.zero;

    public int damage = 100;

    [NonSerialized]
    public bool targetEnemy = true, targetPlayer = true;

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
    }

    void Update()
    {
        // transform.position += direction * Time.deltaTime * speed;
        //gameObject.GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        Destroy(gameObject);
        if (other.CompareTag("Enemy") && targetEnemy)
        {
            other.GetComponent<AbstractEnemy>().health -= damage;
        }
        else if (other.CompareTag("Player") && targetPlayer)
        {
            other.GetComponent<Player>().health -= damage;
        }
    }

    public void Initialize(Vector3 direction, int damage = 100, float speed = 7.5f)
    {
        this.direction = direction;
        this.damage = damage;
        this.speed = speed;
    }
    
        
    
}
