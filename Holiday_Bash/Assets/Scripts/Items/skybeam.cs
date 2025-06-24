using System;
using UnityEngine;

public class skybeam : MonoBehaviour
{
    public int damage;
     
    [NonSerialized]public Player player;
    private float damageTimer = 0f;
    public float damageInterval = 0.5f;
    private bool playerInside = false;

    // Update is called once per frame
    void Update()
    {
        
        if (playerInside)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                damageTimer = -7f;
                player.TakeDamage(damage);
                damageTimer = 0f;
            }

        }
    }
    public void Suicide()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;
        if (other.CompareTag("Player"))
        {
            damageTimer = 7f;
            playerInside = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

}
