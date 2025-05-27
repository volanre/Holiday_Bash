using Unity.VisualScripting;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float speed = 7.5f;
    public Vector3 direction = Vector3.zero;

    public int damage = 100;

    private Player player;

    void Start()
    {
    }

    void Update()
    {
        transform.position += direction * Time.deltaTime * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        Destroy(gameObject);
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<AbstractEnemy>().health -= damage;
        }
        else if (other.CompareTag("Player"))
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
