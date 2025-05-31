using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AbstractCharacter : MonoBehaviour
{
    [NonSerialized] public int health = 999999999;

    /* 
    For all multipliers:
        If bool is true then it is an MULTIPLICATIVE bonus
        If bool is false then it is a ADDITIVE bonus
    */
    [NonSerialized] public List<Tuple<bool, float>> damageMultipliers = new List<Tuple<bool, float>>();
    [NonSerialized] public List<Tuple<bool, float>> defenseMultipliers = new List<Tuple<bool, float>>();
    [NonSerialized] public List<Tuple<bool, float>> moveSpeedMultipliers = new List<Tuple<bool, float>>();
    [NonSerialized] public List<Tuple<bool, float>> fireRateMultipliers = new List<Tuple<bool, float>>();



    [Header("Character Properties")]
    [SerializeField] public string title;
    [SerializeField] protected int maxHealth = 500;
    [SerializeField] public int attack = 100, defense = 100;
    [SerializeField] public float moveSpeed = 5f, fireRate = 0.3f;

    [Header("Projectile Attributes")]
    public float LaunchOffset;
    public ProjectileBehavior ProjectileItem;
    [SerializeField] protected float bulletSpeed;


    [Header("References")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator animator;
    public SoundEffectPlayer noiseMaker;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [Header("Audio Noises")]
    public AudioClip shootingSFX;
    [SerializeField] protected AudioClip deathSound;
    [SerializeField] protected AudioClip defaultImpactSFX;

    public void TakeDamage(int attackedValue)
    {
        health -= CalculateEffectiveDamage(attackedValue);
        spriteRenderer.color = Color.red;
        DamageEffects();
        Invoke("ResetColor", 0.07f);
    }
    public void TakeDamage(int attackedValue, AudioClip impactAudio, float volume = 0.75f)
    {
        health -= CalculateEffectiveDamage(attackedValue);
        spriteRenderer.color = Color.red;
        DamageEffects();
        Invoke("ResetColor", 0.07f);
    }
    public abstract void DamageEffects();
    public abstract void DamageEffects(AudioClip impactAudio, float volume = 0.75f);
    private void ResetColor()
    {
        spriteRenderer.color = Color.white;
    }

    public int CalculateEffectiveDamage(int attackedValue)
    {
        if (attackedValue <= 0) return 0;
        int currentDamage = attackedValue * attackedValue / (attackedValue + defense);
        return currentDamage;
    }

    /// <summary>
    /// Gets the character's effective attack value.
    /// </summary>
    /// <returns></returns>
    public int GetEffectiveAttack()
    {
        float newDamage = attack;
        var multiplicativeOnly = damageMultipliers.Where(x => x.Item1 == true).ToList();
        var additiveOnly = damageMultipliers.Where(x => x.Item1 == false).ToList();
        foreach ((bool type, float value) in multiplicativeOnly)
        {
            newDamage = newDamage * value;
        }
        foreach ((bool type, float value) in additiveOnly)
        {
            newDamage += value;
        }
        return (int)newDamage;
    }
    /// <summary>
    /// Gets the character's effective move speed.
    /// </summary>
    /// <returns></returns>
    public float GetEffectiveSpeed()
    {
        float newSpeed = moveSpeed;
        var multiplicativeOnly = moveSpeedMultipliers.Where(x => x.Item1 == true).ToList();
        var additiveOnly = moveSpeedMultipliers.Where(x => x.Item1 == false).ToList();
        foreach ((bool type, float value) in multiplicativeOnly)
        {
            newSpeed = newSpeed * value;
        }
        foreach ((bool type, float value) in additiveOnly)
        {
            newSpeed += value;
        }
        return newSpeed;
    }
    /// <summary>
    /// Gets the character's effective firerate.
    /// </summary>
    /// <returns></returns>
    public float GetEffectiveFireRate()
    {
        float newFire = fireRate;
        var multiplicativeOnly = fireRateMultipliers.Where(x => x.Item1 == true).ToList();
        var additiveOnly = fireRateMultipliers.Where(x => x.Item1 == false).ToList();
        foreach ((bool type, float value) in multiplicativeOnly)
        {
            newFire = newFire * value;
        }
        foreach ((bool type, float value) in additiveOnly)
        {
            newFire += value;
        }
        return newFire;
    }
    /// <summary>
    /// Gets the character's effective defense value.
    /// </summary>
    /// <returns></returns>
    public float GetEffectiveDefense()
    {
        float newFire = defense;
        var multiplicativeOnly = defenseMultipliers.Where(x => x.Item1 == true).ToList();
        var additiveOnly = defenseMultipliers.Where(x => x.Item1 == false).ToList();
        foreach ((bool type, float value) in multiplicativeOnly)
        {
            newFire = newFire * value;
        }
        foreach ((bool type, float value) in additiveOnly)
        {
            newFire += value;
        }
        return newFire;
    }
    
}
