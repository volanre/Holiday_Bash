using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;

public class FatBlazeling : AbstractEnemy
{   
    /// <summary>
    /// Min is 1, Max is 3. Both inclusive.
    /// </summary>
    private int currentAttackNumber;
    private bool isAttacking = false;

    [Header("Boss Properties")]
    [SerializeField] private int BarrageDamage;

    //At half health spawn a burst of enemies.
    //All attacks now deal burning damage.
    //The lower the hp the more damage burning does.


    void Start()
    {
        currentAttackNumber = Random.Range(1, 4);
    }

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
        //After each attack set the next attack
        //Then here wait for the condition for that attack
        if (!isAttacking) {
            Attack();
        }
        
        

    }

    // public void BarrageAttack()
    // {
    //     damage = BarrageDamage;
    //     int barrageCount = Random.Range(0, 4);
    //     barrageCount += 6;
    //     for (int i = 0; i < barrageCount; i++)
    //     {
    //         Shoot();
    //     }
    //     AttackFinished();
    // }
    IEnumerator BarrageAttack()
    {
        bool attackLeading = Random.value < .5f ? true : false;
        damage = BarrageDamage;
        int barrageCount = Random.Range(0, 4);
        barrageCount += 6;
        Vector3 shootDirection = Vector3.zero;
        float maxAngleRadians = fireSpreadDegrees * Mathf.Deg2Rad;
        
        if (attackLeading)
        {
            ///float timeDelta = ((Vector2)player.transform.position - (Vector2)transform.position) / (//bullet velocity - playervelocity);
            ///Vector3 playerFuturePosition = player.transform.position + new Vector3(player.getVelocity().x, player.getVelocity().y,0) * timeDelta;
            ///Vector3 shootDirection = playerFuturePosition - transform.position;
            ///shootDirection = shootDirection.normalized; //gets direction without the magnitude
            Vector3 playVel = (Vector3)player.getVelocity();
            Vector3 playPos = player.transform.position;
            Vector3 bulPos = transform.position;
            var possibleShootDirection = MathFunctions.GetThrowDirection(playPos, playVel, bulPos, bulletSpeed);
            if (possibleShootDirection == null)
            {
                Vector3 playerFuturePosition = player.transform.position + new Vector3(player.getVelocity().x, player.getVelocity().y, 0) * 0.5f;
                shootDirection = playerFuturePosition - transform.position;
                shootDirection = shootDirection.normalized;
            }
            else
            {
                shootDirection = (Vector3)possibleShootDirection;
            }
        }



        for (int i = 0; i < barrageCount; i++)
        {

            if (!attackLeading)
            {
                shootDirection = player.transform.position - transform.position;
            }
            var upDown = Random.Range(0, 1) == 0 ? Vector3.up : Vector3.down;

            Vector3 newDirection = Vector3.RotateTowards(shootDirection, Vector3.Cross(shootDirection, upDown), maxAngleRadians, 0f);
            newDirection = newDirection.normalized;

            Vector3 center = GetComponent<BoxCollider2D>().bounds.center;

            Vector3 bulletPosition = new Vector3(center.x + launchOffset * newDirection.x, center.y + launchOffset * newDirection.y, 0);

            var bullet = Instantiate(projectileItem, bulletPosition, transform.rotation);
            bullet.targetPlayer = true;
            bullet.targetEnemy = false;
            bullet.Initialize(new Vector3(newDirection.x, newDirection.y, 0), damage, bulletSpeed);
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            yield return new WaitForSeconds(0.095f);
        }
        AttackFinished();
    }
    

    public void Attack()
    {
        if (attackTimer < fireRate) return;
        if (isAttacking) return;
        isAttacking = true;
        if (currentAttackNumber == 1) //Fireball Barrage
        {
            StartCoroutine(BarrageAttack());
        }
        else if (currentAttackNumber == 2) //
        {
            StartCoroutine(BarrageAttack());
        }
        else if (currentAttackNumber == 3)
        {
            StartCoroutine(BarrageAttack());
        }
    }
    public void AttackFinished()
    {
        SetNewFireRate();

        int randomizer = Random.Range(0, 10);
        if (randomizer < 5) //50%
        {
            currentAttackNumber = 1;
        }
        else if (randomizer < 7) //30%
        {
            currentAttackNumber = 2;
        }
        else //20%
        {
            currentAttackNumber = 3;
        }
        
        attackTimer = 0f;
        isAttacking = false;
    }

    private void SetNewFireRate()
    {
        int randomizer = Random.Range(0, 100);
        float fireRateMin = 0.025f;
        float fireRateMax = 0.05f;
        if (randomizer < 1) //1% chance
        {
            fireRateMin = 0.05f;
            fireRateMax = 0.1f;
        }
        else if (randomizer < 10) //9% chance
        {
            fireRateMin = 0.1f;
            fireRateMax = 0.5f;
        }
        else if (randomizer < 30) //20% chance
        {
            fireRateMin = 0.5f;
            fireRateMax = 0.75f;
        }
        else if (randomizer < 60) //30% chance
        {
            fireRateMin = 0.75f;
            fireRateMax = 1f;
        }
        else if (randomizer < 90) //30% chance
        {
            fireRateMin = 1f;
            fireRateMax = 1.25f;
        }
        else if (randomizer < 100) //10% cahnce
        {
            fireRateMin = 2f;
            fireRateMax = 2.5f;
        }
        fireRate = Random.Range(fireRateMin, fireRateMax);
    }

    

    void FixedUpdate()
    {
        if (!initialized) return;
        defaultUpdateBehavior();
    }
}
