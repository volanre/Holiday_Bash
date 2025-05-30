using System;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{

    private float attackTimer = 0f;
    private Vector2 moveDirection = Vector2.zero, shootDirection = Vector2.zero;
    private InputAction move;
    private InputAction attack;
    private AudioClip currentImpactSFX;
    [NonSerialized] public static bool isAlive = true;

    [NonSerialized] public int health;

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
    public PlayerInputActions playerControls;
    public SoundEffectPlayer noiseMaker;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] public HealthBarUI healthBar;

    [Header("Audio Noises")]
    public AudioClip shootingSFX;
    [SerializeField] private  AudioClip deathSound;
    [SerializeField] private AudioClip defaultImpactSFX;



    private void Awake()
    {
        isAlive = true;
        if (playerControls == null) playerControls = new PlayerInputActions();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();
        move.performed += MovePerformed;
        move.canceled += MoveCancelled;

        attack = playerControls.Player.Attack;
        attack.Enable();
        // attack.performed += Attack;
    }

    private void OnDisable()
    {
        move.Disable();
        attack.Disable();
    }

    void Start()
    {
        health = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        healthBar.setCurrentHealth(maxHealth);
    }

    void Update()
    {
        if (!isAlive) return;
        UpdateTimers();

        if (attackTimer >= fireRate)
        {
            Shoot();
        }
    }

    private void MovePerformed(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
        animator.SetBool("isWalking", true);
    }
    private void MoveCancelled(InputAction.CallbackContext context)
    {
        moveDirection = Vector2.zero;
        animator.SetBool("isWalking", false);
    }

    private void Shoot()
    {
        Vector2 attackInput = attack.ReadValue<Vector2>();
        if (attackInput != Vector2.zero)
        {
            attackTimer = 0f;
            shootDirection = attackInput.normalized; // Last active direction

            Vector3 center = GetComponent<BoxCollider2D>().bounds.center;
            Vector3 bulletPosition = new Vector3(center.x + LaunchOffset * shootDirection.x, center.y + (LaunchOffset + 0.3f) * shootDirection.y, 0);
            noiseMaker.PlaySpecificSound(shootingSFX, 0.2f);
            var bullet = Instantiate(ProjectileItem, bulletPosition, transform.rotation);
            bullet.Initialize(new Vector3(shootDirection.x, shootDirection.y, 0), damage, bulletSpeed);
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        }
    }
    public void TakeDamage(int damageTaken)
    {
        DamageEffects(damageTaken);
        noiseMaker.PlaySpecificSound(defaultImpactSFX, 0.9f);


    }
    public void TakeDamage(int damageTaken, AudioClip impactAudio)
    {
        DamageEffects(damageTaken);
        noiseMaker.PlaySpecificSound(impactAudio, 1f);
    }
    private void DamageEffects(int damage)
    {
        health -= damage;
        spriteRenderer.color = Color.red;
        Invoke("ResetColor", 0.05f);
        var clampedHealth = Mathf.Clamp(health, 0, maxHealth);
        healthBar.setCurrentHealth(clampedHealth);

        if (health <= 0)
        {
            Suicide();
        }

    }
    private void Suicide()
    {
        rb.linearVelocity = Vector2.zero;
        OnDisable();
        animator.SetBool("isDead", true);
        if (isAlive)
        {
            noiseMaker.PlayLongSound(deathSound, 1.2f);
            var time = deathSound.length;
            Invoke("CompleteDeath", time);
        }
        //Destroy(gameObject);
        }
    private void CompleteDeath()
    {
        isAlive = false;
    }
    private void ResetColor()
    {
        spriteRenderer.color = Color.white;
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;
        rb.linearVelocity = getVelocity();
    }
    // private void Attack(InputAction.CallbackContext context)
    // {
    //     //Debug.Log("we fired!!");
    //     float posX = transform.position.x;
    //     Vector3 bulletPosition = (transform.position);
    //     Instantiate(ProjectileItem, transform.position, transform.rotation);
    // }
    private void UpdateTimers()
    {
        attackTimer += Time.deltaTime;
    }

    public Vector3Int getPosition()
    {
        return Vector3Int.FloorToInt(transform.position);
    }
    public Vector2 getVelocity()
    {
        return new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

}