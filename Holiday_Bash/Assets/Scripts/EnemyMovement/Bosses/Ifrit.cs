using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ifrit : AbstractEnemy
{
    private int currentAttackNumber;
    [SerializeField] private ProjectileBehavior bulletType1;
    [SerializeField] private skybeam beam;
    [SerializeField] private skybeam breath;
    [SerializeField] private SoundEffectPlayer soundEffectPlayer;
    [SerializeField] private AudioClip EnragedCue;
    [SerializeField] private AudioClip finalCue;
    [SerializeField] private AudioClip SpawningAudio;
    [SerializeField] private GameObject screenDarken;
    [SerializeField] private GameObject teleportSignal;
    [NonSerialized] public MenuAudio musicPlayer;
    private bool isAttacking = false;
    private bool isEnraged = false;
    private bool finalAttack = false;
    private bool dontMove = false;


    void Start()
    {
        musicPlayer.PauseBGMusic();
        soundEffectPlayer.PlaySpecificSound(SpawningAudio, 2f);
        screenDarken.GetComponent<SpriteRenderer>().color = new Color(20f, 2f, 0f, 0.7f);
        screenDarken.SetActive(true);

        initalPause = 30.75f;
        currentAttackNumber = Random.Range(1, 4);
        player.FreezeMovement(false);
        Invoke("BeginFight", 13f);
    }
    void FixedUpdate()
    {
        if (!initialized) return;
        if (dontMove) return;
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
            EnterEnragedMode();
        }
        if (!finalAttack && health < (int)(maxHealth * 0.1f))
        {
            BeginFinalAttack();
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
        if (finalAttack) currentAttackNumber = 1;
        if (currentAttackNumber == 1)
        {
            Debug.Log("Hellrain attack!");
            bool val = Random.Range(0, 10) <= 7 ? true : false;
            if (finalAttack) val = false;
            StartCoroutine(HellRain(val));
        }
        else if (currentAttackNumber == 2)
        {
            Debug.Log("FireFan attack!");
            StartCoroutine(FireFan());
        }
        else if (currentAttackNumber == 3)
        {
            Debug.Log("MagmaShot attack!");
            StartCoroutine(MagmaShot());
        }
        else if (currentAttackNumber == 4)
        {
            Debug.Log("Portalballs attack!");
            StartCoroutine(PortalBalls());
        }
    }
    IEnumerator HellRain(bool clustered)
    {
        float timeDelta;
        int iterations;

        if (clustered)
        {
            iterations = Random.Range(7, 15);
            timeDelta = isEnraged ? Random.Range(.03f, .07f) : Random.Range(.2f, .3f);
            if (isEnraged) iterations += 10;
        }
        else
        {
            iterations = (int)Random.Range(0.1f * room.roomFloor.Count, 0.2f * room.roomFloor.Count);
            timeDelta = isEnraged ? Random.Range(.003f, .01f) : Random.Range(.03f, .07f);
            if (isEnraged) iterations += 15;
        }

        HashSet<Vector2Int> usedSpots = new HashSet<Vector2Int>();
        for (int i = 0; i < iterations; i++)
        {
            bool done = false;
            Vector2Int position = room.roomCenter;

            int index = 0;
            while (!done)
            {
                int randomIndex;
                int randomDegree;
                if (clustered)
                {
                    randomDegree = Random.Range(-90, 90);
                    randomIndex = Random.Range(0, 5);
                }
                else
                {
                    randomDegree = Random.Range(0, 360);
                    randomIndex = Random.Range(0, 15);
                }
                Vector2 dir = player.getVelocity().normalized;
                if (dir == Vector2.zero) dir = Vector2.up;
                var newDirection = Quaternion.Euler(0, 0, randomDegree) * dir;
                Vector2 pos = player.transform.position + newDirection.normalized * randomIndex;
                position = new Vector2Int((int)pos.x, (int)pos.y);

                if (!usedSpots.Contains(position) && room.roomFloor.Contains(position))
                {
                    done = true;
                    position = position + 4 * Vector2Int.up;
                    usedSpots.Add(position);
                }
                else if (index >= 80)
                {
                    yield break;
                }
                index++;
            }
            var newBeam = Instantiate(beam, new Vector3(position.x, position.y), Quaternion.identity);
            newBeam.damage = 20;
            newBeam.player = player;
            yield return new WaitForSeconds(timeDelta);
        }
        AttackFinished();
    }
    IEnumerator FireFan()
    {
        float timeDelta = 0.28f;
        int iterations = isEnraged ? 3 : 1;
        for (int i = 0; i < iterations; i++) {

            bool done = false;
            Vector2Int position = room.roomCenter;

            int index = 0;
            while (!done)
            {
                int randomIndex = 4;
                int randomDegree = Random.Range(0, 360);
                Vector2 dir = player.getVelocity().normalized;
                if (dir == Vector2.zero) dir = Vector2.up;
                var newDirection = Quaternion.Euler(0, 0, randomDegree) * dir;
                Vector2 pos = player.transform.position + newDirection.normalized * randomIndex;
                position = new Vector2Int((int)pos.x, (int)pos.y);

                if (room.roomFloor.Contains(position))
                {
                    done = true;
                }
                else if (index >= 80)
                {
                    yield break;
                }
                index++;
            }
            Vector3 spawnPoint = new Vector3(position.x, position.y);
            transform.position = Vector3.zero;

            Instantiate(teleportSignal, spawnPoint, Quaternion.identity);
            yield return new WaitForSeconds(0.45f);

            transform.position = spawnPoint;
            dontMove = true;
            rb.linearVelocity = Vector2.zero;

            yield return new WaitForSeconds(0.2f);

            Vector3 flameDirection = player.transform.position - transform.position;


            spawnPoint = spawnPoint + flameDirection.normalized * 4f;

            var flame = Instantiate(breath, spawnPoint, Quaternion.identity);
            flame.damage = 110;
            flame.player = player;

            float flameAngleDegrees = Mathf.Atan2(flameDirection.y, flameDirection.x) * Mathf.Rad2Deg;
            flame.transform.rotation = Quaternion.Euler(0, 0, flameAngleDegrees);
            yield return new WaitForSeconds(timeDelta);
            dontMove = false;
        }
        AttackFinished();
    }
    IEnumerator MagmaShot()
    {
        float timeDelta = 0.095f;
        int iterations = Random.Range(15, 25);
        Vector3 newDirection = player.transform.position - transform.position;
        for (int p = 0; p < iterations; p++)
        {
            if(p % 5 == 0) {newDirection = player.transform.position - transform.position;} 
            newDirection = newDirection.normalized;
            float spawnRange = Random.Range(-0.75f, 0.75f);
            Vector3 realSpawn = transform.position + spawnRange * new Vector3(-newDirection.x, newDirection.y);
            
            var missile = Instantiate(projectileItem, realSpawn, Quaternion.identity);
            missile.targetPlayer = true;
            missile.targetEnemy = false;
            float missileSpeed = isEnraged ? 12f : 11f;
            missile.Initialize(newDirection, (int)(GetEffectiveAttack() * 0.5f), missileSpeed, false);
            Physics2D.IgnoreCollision(missile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            yield return new WaitForSeconds(timeDelta);
        }
        
        AttackFinished();
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
            //Debug.Log("Portal at: " + spawnPoint + ", player at: " + player.transform.position);

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
                missile.Initialize(-newDirection.normalized, (int)(GetEffectiveAttack() * 0.7f), missileSpeed, false);
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
        if (randomizer < 25) //25%
        {
            if(currentAttackNumber == 1) { currentAttackNumber = 4; }
            else{currentAttackNumber = 1;}
        }
        else if (randomizer < 60) //35%
        {
            if(currentAttackNumber == 2) { currentAttackNumber = 3; }
            else{currentAttackNumber = 2;}
        }
        else if (randomizer < 80) //20%
        {
            currentAttackNumber = 3;
        }
        else //20%
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
            fireRateMin = 3f;
            fireRateMax = 3.5f;
            
        }
        else if (randomizer < 7) //6% chance
        {
            fireRateMin = 1.25f;
            fireRateMax = 1.75f;
            
        }
        else if (randomizer < 20) //13% chance
        {
            fireRateMin = 1f;
            fireRateMax = 1.25f;
        }
        else if (randomizer < 45) //25% chance
        {
            fireRateMin = .8f;
            fireRateMax = 1f;
        }
        else if (randomizer < 85) //40% chance
        {
            fireRateMin = 0.1f;
            fireRateMax = 0.8f;
        }
        else if (randomizer < 100) //15% cahnce
        {
            fireRateMin = 0.05f;
            fireRateMax = 0.1f;
        }
        fireRate = Random.Range(fireRateMin, fireRateMax);
        if (finalAttack) fireRate = 0.05f;
    }
    private void EnterEnragedMode()
    {
        if (isEnraged) return;
        isEnraged = true;
        damageMultipliers.Add(new Tuple<bool, float>(true, 1.5f));

        soundEffectPlayer.PlaySpecificSound(EnragedCue, 1.3f);
        walkSpeed *= 2.8f;
        speed *= 1.2f;
        shootingRange += 2f;
    }
    private void BeginFinalAttack()
    {
        if (finalAttack) return;
        finalAttack = true;
        soundEffectPlayer.PlaySpecificSound(finalCue, 2f);
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
    public void BeginFight()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetBool("spawn", true);
        Invoke("BrightenScreen", 3f);
    }
    private void BrightenScreen()
    {
        player.FreezeMovement(true);
        musicPlayer.UnpauseBGMusic();
        intialPauseTimer = 0f;
        initalPause = 1.8f;
        screenDarken.SetActive(false);
    }
}
