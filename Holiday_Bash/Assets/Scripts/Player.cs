using System;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class Player : AbstractCharacter
{

    private float attackTimer = 0f;
    private Vector2 moveDirection = Vector2.zero, shootDirection = Vector2.zero;
    private InputAction moveAction;
    private InputAction attackAction;
    private AudioClip currentImpactSFX;

    [NonSerialized] public Effects effectsObject;
    [NonSerialized] public static bool isAlive = true;

    [Header("Audio Noises")]
    public AudioClip shootingSFX;
    [SerializeField] protected AudioClip deathSound;
    [SerializeField] protected AudioClip defaultImpactSFX;

    [Header("References")]
    public ProjectileBehavior projectileItem;
    public SoundEffectPlayer noiseMaker;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator animator;

    public PlayerInputActions playerControls;
    [SerializeField] public HealthBarUI healthBar;

    

    private void Awake()
    {
        isAlive = true;
        if (playerControls == null) playerControls = new PlayerInputActions();
        if (animator == null) animator = GetComponent<Animator>();
        effectsObject = new Effects(this);
    }

    private void OnEnable()
    {
        moveAction = playerControls.Player.Move;
        moveAction.Enable();
        moveAction.performed += MovePerformed;
        moveAction.canceled += MoveCancelled;

        attackAction = playerControls.Player.Attack;
        attackAction.Enable();
        // attack.performed += Attack;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        attackAction.Disable();
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
        effectsObject.UpdateEffects();

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
        Vector2 attackInput = attackAction.ReadValue<Vector2>();
        if (attackInput != Vector2.zero)
        {
            attackTimer = 0f;
            shootDirection = attackInput.normalized; // Last active direction

            Vector3 center = GetComponent<BoxCollider2D>().bounds.center;
            Vector3 bulletPosition = new Vector3(center.x + launchOffset * shootDirection.x, center.y + (launchOffset + 0.3f) * shootDirection.y, 0);
            noiseMaker.PlaySpecificSound(shootingSFX, 0.2f);
            var bullet = Instantiate(projectileItem, bulletPosition, transform.rotation);
            bullet.Initialize(new Vector3(shootDirection.x, shootDirection.y, 0), GetEffectiveAttack(), bulletSpeed);
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        }
    }
    // public void TakeDamage(int damageTaken)
    // {
    //     DamageEffects(damageTaken);
    //     noiseMaker.PlaySpecificSound(defaultImpactSFX, 0.75f);
    public override void DamageEffects()
    {
        noiseMaker.PlaySpecificSound(defaultImpactSFX, 0.75f);
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
    private void FixedUpdate()
    {
        if (!isAlive) return;
        rb.linearVelocity = getVelocity();
    }
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
        return new Vector2(moveDirection.x * GetEffectiveSpeed(), moveDirection.y * GetEffectiveSpeed());
    }

}