using Unity.VisualScripting;
using UnityEngine;

public class FirespriteEnemy : AbstractEnemy
{
    [SerializeField]
    private SoundEffectPlayer soundEffect;

    void Update()
    {
        updateTimers();
        if (!checkInitialized()) return;

        checkIfDead();
        if (isDead)
        {
            boomTimer -= Time.deltaTime;
            if (boomTimer <= 0)
            {
                suicide();
            }
        }
        
        if (inShootingRange())
        {
            if (attackTimer >= fireRate)
            {
                // Debug.Log("Firesprite shoots");
                Shoot();
                attackTimer = 0f;
                if (soundEffect != null)
                {
                    soundEffect.PlayLoadedSound(0.2f);
                }

            }
        }
    }

    private void FixedUpdate()
    {
        if (!initialized) return;
        defaultUpdateBehavior();
    }

    public override void DamageEffects()
    {
        return;
    }
}
