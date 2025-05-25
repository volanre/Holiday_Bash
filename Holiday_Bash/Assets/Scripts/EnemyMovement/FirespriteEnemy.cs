using UnityEngine;

public class FirespriteEnemy : AbstractEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        updateTimers();
        if (playerDetected())
        {
            if (attackTimer >= fireRate)
            {
                Debug.Log("Firesprite shoots");
                Shoot();
            }
        }
    }
    
}
