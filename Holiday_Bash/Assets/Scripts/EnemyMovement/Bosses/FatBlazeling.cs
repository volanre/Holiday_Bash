using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;

public class FatBlazeling : AbstractEnemy
{
    //note to self: playing the enragedcue several times back to back sounds like a dragon


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
    private bool minionsSummoned = false;


    [Header("Boss Properties")]
    [SerializeField] private int barrageAttackValue;
    [SerializeField] private int circlePulseAttackValue;
    [SerializeField] private int spinnyTopAttackValue;

    //At half health spawn a burst of enemies.
    //Barrage attack now deals burning damage
    //Spacing between pulse attack shrinks
    //Spinny attack rotates faster
    //General stat buffs (speed, firerate)

    //at minion summoning possible add a new laser beam attack

    /*
    maxHealth = 3000;
    defense = 100;
    speed = 3;
    fireRate = 0.5f;
    walkSpeed = 1.5f;
    detectionRadius = 20;
    shootingRange = 5;
    */


    void Start()
    {
        currentAttackNumber = Random.Range(1, 4);
        if (maxHealth == 0) maxHealth = 3000;
        if (defense == 0) defense = 100;
        if (speed == 0) speed = 3;
        if (fireRate == 0) fireRate = 0.5f;
        if (walkSpeed == 0) walkSpeed = 1.5f;
        if (detectionRadius == 0) detectionRadius = 20;
        if (shootingRange == 0) shootingRange = 5;
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
                GameObject[] clones = GameObject.FindGameObjectsWithTag("Enemy");
                clones.Union(GameObject.FindGameObjectsWithTag("Enemy_Bullet"));
                foreach (GameObject clone in clones)
                {
                    Destroy(clone);
                }

                suicide();
            }
        }

        if (!minionsSummoned && health < (int)(maxHealth * 0.65f))
        {
            minionsSummoned = true;
            SummonMinions();
        }
        if (!isEnraged && health < (int)(maxHealth * 0.35f))
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
            if (isEnraged) bullet.setEffect("burning", 1, 5);
            bullet.Initialize(new Vector3(newDirection.x, newDirection.y, 0), CalculateEffectiveDamage(barrageAttackValue), bulletSpeed, true);
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            yield return new WaitForSeconds(0.095f);
        }
        AttackFinished();
    }

    IEnumerator CirclePulseAttack()
    {
        float timeDelta = isEnraged ? 0.4f : 0.5f;
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
                    bullet.Initialize(new Vector3(newDirection.x, newDirection.y, 0), CalculateEffectiveDamage(circlePulseAttackValue), bulletSpeed, false);
                    bullet.GetComponent<SpriteRenderer>().color = Color.black;
                    Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                }
            }
            yield return new WaitForSeconds(timeDelta);
        }
        AttackFinished();
    }

    IEnumerator SpinnyTopAttack()
    {
        float timeDelta = 0.11f;
        int iterations = Random.Range(30, 45);
        for (int i = 0; i < iterations; i++)
        {

            foreach (var dir in Direction2D.eightDirectionsList)
            {
                Vector3 direction = (Vector3)(Vector3Int)dir;
                float spacing = isEnraged ? 8f : 6.3f;
                Vector3 newDirection = Quaternion.Euler(0, 0, i * spacing) * direction;
                newDirection = newDirection.normalized;

                Vector3 center = GetComponent<BoxCollider2D>().bounds.center;

                Vector3 bulletPosition = new Vector3(center.x + launchOffset * newDirection.x, center.y + launchOffset * newDirection.y, 0);

                var bullet = Instantiate(projectileItem, bulletPosition, transform.rotation);
                bullet.targetPlayer = true;
                bullet.targetEnemy = false;
                bullet.Initialize(new Vector3(newDirection.x, newDirection.y, 0), CalculateEffectiveDamage(spinnyTopAttackValue), bulletSpeed, true);
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
        if (currentAttackNumber == 1)
        {
            StartCoroutine(BarrageAttack());
        }
        else if (currentAttackNumber == 2)
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
        int randomizer = isEnraged ? Random.Range(0,84) : Random.Range(0, 100);

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
        soundEffectPlayer.PlaySpecificSound(EnragedCue, 1f);
        isEnraged = true;
        walkSpeed *= 2.3f;
        speed *= 1.2f;
        shootingRange -= 1f + 1.3f;   
    }

    private void SummonMinions()
    {
        minionsSummoned = true;
        shootingRange += 1.3f;
        soundEffectPlayer.PlaySpecificSound(EnragedCue, 0.7f);
        foreach (var dir in Direction2D.cardinalDirectionsList)
        {
            var position = transform.position + new Vector3(1.5f * dir.x, 1.5f * dir.y, 0);
            var thisMinoin = Instantiate(minion, position, transform.rotation);
            minionsList.Add(thisMinoin);
            thisMinoin.player = player;
            thisMinoin.room = room;
            thisMinoin.health = (int)(thisMinoin.health * 0.6f);
            thisMinoin.attack = (int)(thisMinoin.attack * 0.4f);
            thisMinoin.speed *= 1.25f;
            thisMinoin.fireRate *= 1.2f;
            thisMinoin.detectionRadius = 20f;
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), thisMinoin.GetComponent<Collider2D>());
        }
    }
    void FixedUpdate()
    {
        if (!initialized) return;
        defaultUpdateBehavior();
    }

    public override void DamageEffects()
    {
        return;
    }


    
}
