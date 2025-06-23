using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;

public class Ifrit : AbstractEnemy
{
    private int currentAttackNumber;
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
    }

    IEnumerator MagmaShot()
    {
        return null;
    }

    IEnumerator FireFan()
    {
        return null;
    }

    IEnumerator HellRain()
    {
        return null;
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
