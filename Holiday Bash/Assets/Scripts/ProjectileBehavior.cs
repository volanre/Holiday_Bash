using Unity.VisualScripting;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float speed = 7.5f;
    public Vector3 direction = Vector3.zero;

    private Player player;

    void Start()
    {
    }

    void Update()
    {
        transform.position += direction * Time.deltaTime * speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
    }
    
        
    
}
