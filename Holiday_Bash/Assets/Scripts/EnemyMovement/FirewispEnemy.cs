using UnityEngine;
using UnityEngine.EventSystems;

public class FirewispEnemy : AbstractEnemy
{
    [SerializeField]
    private SoundEffectPlayer soundEffect;

    void Start()
    {
        if(animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        updateTimers();
        if (!checkInitialized()) return;

        if (!isCharging) animator.SetBool("isMoving", false);
        else if (isCharging) animator.SetBool("isMoving", true);

        checkIfDead();
        if (isDead)
        {
            boomTimer -= Time.deltaTime;
            if (boomTimer <= 0)
            {
                suicide();
            }
        }
        
        
    }

    private void FixedUpdate()
    {
        if (!initialized) return;
        defaultUpdateBehavior();

        if (inShootingRange())
        {
            if (!isCharging && attackTimer >= fireRate)
            {
                if (Random.value > 0.007f)
                {
                    isCharging = true;
                    forwardsCharging = false;
                }
            }
            if (isCharging) Charge();
        }
        else
        {
            isCharging = false;
            forwardsCharging = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;
        if (other.CompareTag("Player_Bullet"))
        {
            health -= other.GetComponent<ProjectileBehavior>().damage;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().health -= damage;
        }
    }
}
