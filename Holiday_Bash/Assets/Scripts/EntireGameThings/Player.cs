using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class Player : AbstractCharacter
{

    private float attackTimer = 0f, dashTimer = 0f;
    private Vector2 moveDirection = Vector2.zero, shootDirection = Vector2.zero, dashDirection = Vector2.left;
    private Vector2Int lastPosition = Vector2Int.zero;
    private bool dashAvailable = true, isDashing = false;
    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction interactAction;
    private InputAction dashAction;
    private InputAction meleeAction;
    private AudioClip currentImpactSFX;
    [NonSerialized] public Dictionary<Vector2Int,int> playerFloodField;
    [NonSerialized] public Dictionary<Vector2Int,int> playerClearenceMap;
    [NonSerialized] public Effects effectsObject;
    [NonSerialized] public static bool isAlive = true;

    [Header("Audio Noises")]
    public AudioClip shootingSFX;
    [SerializeField] protected AudioClip deathSound;
    [SerializeField] protected AudioClip defaultImpactSFX;

    [Header("References")]
    [SerializeField] public ProjectileBehavior projectileItem;
    [SerializeField] public SoundEffectPlayer noiseMaker;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator animator;
    [SerializeField] public PlayerInputActions playerControls;
    [SerializeField] public HealthBarUI healthBar;
    [SerializeField] public RoomCollection currentRoom;
    [Header("Extra Player Items")]
    [SerializeField] public float dashCooldown = 0.27f;
    [SerializeField] public float dashLength = 0.15f;



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

        interactAction = playerControls.Player.Interact;
        interactAction.Enable();

        dashAction = playerControls.Player.Dash;
        dashAction.Enable();
        dashAction.performed += DashPerformed;
        dashAction.canceled += DashCancelled;

        meleeAction = playerControls.Player.Melee;
        meleeAction.Enable();

        attackAction = playerControls.Player.Attack;
        attackAction.Enable();
        // attack.performed += Attack;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        attackAction.Disable();
        interactAction.Disable();
        dashAction.Disable();
        meleeAction.Disable();
    }
    public bool GetInteraction()
    {
        return interactAction.IsPressed();
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
    private void FixedUpdate()
    {
        if (!isAlive) return;
        if (!isDashing)
        {
            rb.linearVelocity = getVelocity();
        }
    }

    private void MovePerformed(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
        dashDirection = moveDirection;
        animator.SetBool("isWalking", true);
    }
    private void MoveCancelled(InputAction.CallbackContext context)
    {
        moveDirection = Vector2.zero;
        animator.SetBool("isWalking", false);
    }

    private void DashPerformed(InputAction.CallbackContext context)
    {
        if (dashAvailable && (dashTimer > dashCooldown))
        {
            Vector2 attackInput = attackAction.ReadValue<Vector2>();
            if (attackInput != Vector2.zero)
            {
                return; //no dashing while attacking
            }

            attackAction.Disable();
            meleeAction.Disable();
            dashAvailable = false;
            isDashing = true;
            dashTimer = -100f;
            float dashSpeed = speed * 4;
            Vector2 dashVelocity = new Vector2(dashDirection.x * dashSpeed, dashDirection.y * dashSpeed);
            rb.linearVelocity = dashVelocity;
            Invoke("EndDash", dashLength);
            
            //animator.SetBool("isWalking", true);
        }
    }
    private void EndDash()
    {
        attackAction.Enable();
        meleeAction.Enable();
        dashTimer = 0f;
        isDashing = false;
        rb.linearVelocity = getVelocity();
    }
    private void DashCancelled(InputAction.CallbackContext context)
    {
        dashAvailable = true;
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
    private void UpdateTimers()
    {
        attackTimer += Time.deltaTime;
        dashTimer += Time.deltaTime;
    }

    public Vector3Int getPosition()
    {
        return Vector3Int.FloorToInt(transform.position);
    }
    public Vector2 getVelocity()
    {
        return new Vector2(moveDirection.x * GetEffectiveSpeed(), moveDirection.y * GetEffectiveSpeed());
    }
    public void UpdateFloodField()
    {
        if ((currentRoom.roomType.Equals("fight") || currentRoom.roomType.Equals("boss") || currentRoom.roomType.Equals("elite_fight")) && !currentRoom.status.Equals("cleared"))
        {
            Vector2Int currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);

            if (lastPosition != currentPosition)
            {
                lastPosition = currentPosition;
                var roomGraph = new RoomGraph(currentRoom.roomFloor);
                playerFloodField = roomGraph.RunWeightedBFS(currentPosition, currentRoom.propPositions);
                playerClearenceMap = roomGraph.CreateClearenceMap(playerFloodField);
            }
        }
    }

}