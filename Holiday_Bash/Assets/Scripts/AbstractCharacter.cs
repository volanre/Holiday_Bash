using System;
using UnityEngine;

public abstract class AbstractCharacter : MonoBehaviour
{
    [NonSerialized] public int health = 999999999;

    [Header("Character Properties")]
    [SerializeField] protected int maxHealth = 500;
    [SerializeField] protected int damage = 100;
    [SerializeField] protected float moveSpeed = 5f, fireRate = 0.3f;

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

    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        spriteRenderer.color = Color.red;
        DamageEffects();
        Invoke("ResetColor", 0.07f);
    }
    public void TakeDamage(int damageTaken, AudioClip impactAudio, float volume = 0.75f)
    {
        health -= damageTaken;
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
    
}
