using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;

public class Ifrit : AbstractEnemy
{
    private int currentAttackNumber;
    [SerializeField] private ProjectileBehavior bulletType1;
    [SerializeField] private SoundEffectPlayer soundEffectPlayer;
    [SerializeField] private AudioClip EnragedCue;
    private bool isAttacking = false;
    private bool isEnraged = false;
    

    void Start()
    {
        initalPause = 1.75f;
        currentAttackNumber = Random.Range(1, 4);
    }
    void FixedUpdate()
    {
        if (!initialized) return;
        defaultUpdateBehavior();
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


    public void Attack()
    {
        if (attackTimer < fireRate) return;
        if (isAttacking) return;
        isAttacking = true;
        currentAttackNumber = 4; //delete later
        if (currentAttackNumber == 1)
        {
            StartCoroutine(HellRain());
        }
        else if (currentAttackNumber == 2)
        {
            StartCoroutine(FireFan());
        }
        else if (currentAttackNumber == 3)
        {
            StartCoroutine(MagmaShot());
        }
        else if (currentAttackNumber == 4)
        {
            StartCoroutine(PortalBalls());
        }
    }

    IEnumerator FireFan()
    {
        return null;
    }

    IEnumerator HellRain()
    {
        return null;
    }
    IEnumerator MagmaShot()
    {
        return null;
    }
    IEnumerator PortalBalls()
    {
        float timeDelta = isEnraged ? Random.Range(.2f, .4f) : Random.Range(.3f, .5f);
        int iterations = Random.Range(5, 7);
        for (int i = 0; i < iterations; i++)
        {
            float randomDist = Random.Range(5, 8);
            bool done = false;
            int index = 0;
            Vector3 spawnPoint = Vector3.zero;
            Vector3 newDirection = Vector3.zero;
            while (!done)
            {
                float randomDegree = Random.Range(0, 360);
                newDirection = Quaternion.Euler(0, 0, randomDegree) * Vector3.up;
                spawnPoint = player.transform.position + newDirection.normalized * randomDist;
                if (room.roomFloor.Contains(new Vector2Int((int)spawnPoint.x, (int)spawnPoint.y)))
                {
                    done = true;
                }
                index++;
                if (index >= 50)
                {
                    Debug.Log("Portalballs failed :(");
                    yield break;
                }
            }
            Debug.Log("Portal at: " + spawnPoint + ", player at: " + player.transform.position);

            int missileCount = Random.Range(4, 7);
            for (int p = 0; p < missileCount; p++)
            {
                float spawnRange = Random.Range(-1f, 1f);
                Vector3 realSpawn = spawnPoint + spawnRange * new Vector3(-newDirection.x, newDirection.y);
                if (!room.roomFloor.Contains(new Vector2Int((int)realSpawn.x, (int)realSpawn.y)))
                {
                    continue;
                }
                var missile = Instantiate(bulletType1, realSpawn, Quaternion.identity);
                missile.targetPlayer = true;
                missile.targetEnemy = false;
                float missileSpeed = isEnraged ? 10.5f : 9f;
                missile.Initialize(-newDirection.normalized, 90, missileSpeed, false);
                Physics2D.IgnoreCollision(missile.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                yield return new WaitForSeconds(0.095f);
            }
            yield return new WaitForSeconds(timeDelta);
        }
        AttackFinished();
    }





    public void AttackFinished()
    {
        SetNewFireRate();

        int randomizer = Random.Range(0, 100);
        if (randomizer < 30) //30%
        {
            currentAttackNumber = 1;
        }
        else if (randomizer < 65) //35%
        {
            currentAttackNumber = 2;
        }
        else if (randomizer < 85) //20%
        {
            currentAttackNumber = 3;
        }
        else //15%
        {
            currentAttackNumber = 4;
        }
        attackTimer = 0f;
        isAttacking = false;
    }

    private void SetNewFireRate()
    {
        int randomizer = isEnraged ? Random.Range(0, 84) : Random.Range(0, 100);

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
        throw new System.NotImplementedException();
    }
    protected override void targetPlayer()
    {
        if (inShootingRange() && !isCharging)
        {
            drift();
            return;
        }
        var dir = player.transform.position - transform.position;
        moveDirection = new Vector2(dir.x, dir.y);
        moveDirection = moveDirection.normalized;

        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, moveDirection * speed, 0.8f);
    }

    public override void DamageEffects()
    {
        return;
    }
}
