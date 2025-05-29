using Unity.VisualScripting;
using UnityEngine;

public class FirespriteEnemy : AbstractEnemy
{
    [SerializeField]
    private SoundEffectPlayer soundEffect;

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
                if (soundEffect != null)
                {
                    soundEffect.playSound();
                }

            }
        }
    }

    private void FixedUpdate()
    {
        defaultUpdateBehavior();
    }
}
