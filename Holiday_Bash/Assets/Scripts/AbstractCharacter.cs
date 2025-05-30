using System;
using UnityEngine;

public abstract class AbstractCharacter : MonoBehaviour
{
    [NonSerialized] public int health = 999999999;

    [Header("Player Properties")]
    [SerializeField] private int maxHealth = 500;
    [SerializeField] private int damage = 100;
    [SerializeField] private float moveSpeed = 5f, fireRate = 0.3f;

    [Header("Projectile Attributes")]
    public float LaunchOffset;
    public ProjectileBehavior ProjectileItem;
    [SerializeField] float bulletSpeed;


    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    public SoundEffectPlayer noiseMaker;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Audio Noises")]
    public AudioClip shootingSFX;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip defaultImpactSFX;

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
