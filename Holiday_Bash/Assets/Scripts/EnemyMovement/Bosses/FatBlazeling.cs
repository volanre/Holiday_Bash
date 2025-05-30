using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class FatBlazeling : AbstractEnemy
{   
    /// <summary>
    /// Min is 1, Max is 3. Both inclusive.
    /// </summary>
    private int currentAttackNumber;
    [SerializeField] private SoundEffectPlayer soundEffectPlayer;
    [SerializeField] private AudioClip EnragedCue;
    [SerializeField] private AbstractEnemy minion;

    private List<AbstractEnemy> minionsList = new List<AbstractEnemy>();
    private bool isAttacking = false;
    private bool isEnraged = false;


    [Header("Boss Properties")]
    [SerializeField] private int BarrageDamage;
    [SerializeField] private int circlePulseDamage;
    [SerializeField] private int spinnyTopDamage;

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
                foreach (AbstractEnemy guy in minionsList)
                {
                    Destroy(guy);
                }
                suicide();
            }
        }

        if (!isEnraged && health < (maxHealth / 2))
        {
            isEnraged = true;
            EnterEnragedMode();
        }

        if (!isAttacking)
        {
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
            bullet.Initialize(new Vector3(newDirection.x, newDirection.y, 0), damage, bulletSpeed, true);
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            yield return new WaitForSeconds(0.095f);
        }
        AttackFinished();
    }

    IEnumerator CirclePulseAttack()
    {
        damage = circlePulseDamage;
        float timeDelta = 0.5f;
        for (int i = 0; i < 5; i++)
        {
            int randomDegree = Random.Range(0, 90);
            foreach (var dir in Direction2D.eightDirectionsList)
            {
                Vector3 direction = (Vector3)(Vector3Int)dir;
                for (int p = 0; p < 2; p++)
                {
                    Vector3 newDirection = Quaternion.Euler(0, 0, (p * 22.5f) + randomDegree) * direction;

                    newDirection = newDirection.normalized;

                    Vector3 center = GetComponent<BoxCollider2D>().bounds.center;

                    Vector3 bulletPosition = new Vector3(center.x + launchOffset * newDirection.x, center.y + launchOffset * newDirection.y, 0);

                    var bullet = Instantiate(projectileItem, bulletPosition, transform.rotation);
                    bullet.targetPlayer = true;
                    bullet.targetEnemy = false;
                    bullet.Initialize(new Vector3(newDirection.x, newDirection.y, 0), damage, bulletSpeed, false);
                    bullet.GetComponent<SpriteRenderer>().color = Color.black;
                    Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                }
            }            
            yield return new WaitForSeconds(timeDelta);
        }
        AttackFinished();
    }

    IEnumerator SpinnyTopAttack() {
        damage = spinnyTopDamage;
        float timeDelta = 0.11f;
        int iterations = Random.Range(30, 45);
        for (int i = 0; i < iterations; i++)
        {
            
            foreach (var dir in Direction2D.eightDirectionsList)
            {
                Vector3 direction = (Vector3)(Vector3Int)dir;
                Vector3 newDirection = Quaternion.Euler(0, 0, i * 6.3f) * direction;
                newDirection = newDirection.normalized;

                Vector3 center = GetComponent<BoxCollider2D>().bounds.center;

                Vector3 bulletPosition = new Vector3(center.x + launchOffset * newDirection.x, center.y + launchOffset * newDirection.y, 0);

                var bullet = Instantiate(projectileItem, bulletPosition, transform.rotation);
                bullet.targetPlayer = true;
                bullet.targetEnemy = false;
                bullet.Initialize(new Vector3(newDirection.x, newDirection.y, 0), damage, bulletSpeed, true);
                Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
            
            yield return new WaitForSeconds(timeDelta);
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
            StartCoroutine(CirclePulseAttack());
        }
        else if (currentAttackNumber == 3)
        {
            StartCoroutine(SpinnyTopAttack());
        }
    }
    public void AttackFinished()
    {
        SetNewFireRate();

        int randomizer = Random.Range(0, 100);
        if (randomizer < 50) //50%
        {
            currentAttackNumber = 1;
        }
        else if (randomizer < 85) //35%
        {
            currentAttackNumber = 2;
        }
        else //15%
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
        else if (randomizer < 7) //6% chance
        {
            fireRateMin = 0.1f;
            fireRateMax = 0.5f;
        }
        else if (randomizer < 20) //13% chance
        {
            fireRateMin = 0.75f;
            fireRateMax = 1f;
        }
        else if (randomizer < 45) //25% chance
        {
            fireRateMin = 1f;
            fireRateMax = 1.5f;
        }
        else if (randomizer < 85) //40% chance
        {
            fireRateMin = 1.5f;
            fireRateMax = 2f;
        }
        else if (randomizer < 100) //15% cahnce
        {
            fireRateMin = 3f;
            fireRateMax = 3.5f;
        }
        fireRate = Random.Range(fireRateMin, fireRateMax);
    }

    private void EnterEnragedMode()
    {
        //change the sprite to its enraged version
        soundEffectPlayer.PlaySpecificSound(EnragedCue, 1.5f);
        isEnraged = true;
        walkSpeed *= 2;
        speed *= 1.3f;
        foreach (var dir in Direction2D.cardinalDirectionsList)
        {
            var position = transform.position + new Vector3(1.5f * dir.x, 1.5f * dir.y, 0);
            var thisMinoin = Instantiate(minion, position, transform.rotation);
            minionsList.Add(thisMinoin);
            thisMinoin.player = player;
            thisMinoin.room = room;
            thisMinoin.damage /= 2;
            thisMinoin.speed *= 0.8f;
            thisMinoin.fireRate *= 1.2f;
            thisMinoin.detectionRadius = 20f;
        }
    }

    void FixedUpdate()
    {
        if (!initialized) return;
        defaultUpdateBehavior();
    }
}
