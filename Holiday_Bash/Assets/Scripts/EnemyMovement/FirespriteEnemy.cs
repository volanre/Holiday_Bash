using Unity.VisualScripting;
using UnityEngine;

public class FirespriteEnemy : AbstractEnemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireRate = fireRate * 0.75f;
    }

    // Update is called once per frame
    void Update()
    {
        checkIfDead();
        if (isDead)
        {
            boomTimer -= Time.deltaTime;
            if (boomTimer <= 0)
            {
                suicide();
            }
        }
        updateTimers();
        if (inShootingRange())
        {
            if (attackTimer >= fireRate)
            {
                // Debug.Log("Firesprite shoots");
                Shoot();
            }
        }
    }

    private void FixedUpdate()
    {
        drift();
        targetingPlayer();
    }
}
