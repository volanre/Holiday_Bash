using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;

    [NonSerialized]
    public int health = 500;
    public float moveSpeed = 5f;
    public float fireRate = 0.3f;
    public PlayerInputActions playerControls;


    private Vector2 moveDirection = Vector2.zero, shootDirection = Vector2.zero;
    private InputAction move;
    private InputAction attack;
    private float attackTimer = 0f;

    public ProjectileBehavior ProjectileItem;
    public float LaunchOffset;
    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

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

    }

    void Update()
    {
        updateTimers();

        moveDirection = move.ReadValue<Vector2>();



        if (attackTimer >= fireRate)
        {
            Shoot();
        }


    }

    private void Shoot()
    {
        Vector2 attackInput = attack.ReadValue<Vector2>();
        if (attackInput != Vector2.zero)
        {
            attackTimer = 0f;
            shootDirection = attackInput.normalized; // Last active direction

            Vector3 center = GetComponent<BoxCollider2D>().bounds.center;
            Vector3 bulletPosition = new Vector3(center.x + LaunchOffset * shootDirection.x, center.y + (LaunchOffset+0.3f) * shootDirection.y, 0);

            var bullet = Instantiate(ProjectileItem, bulletPosition, transform.rotation);
            bullet.Initialize(new Vector3(shootDirection.x, shootDirection.y, 0));
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);

    }
    // private void Attack(InputAction.CallbackContext context)
    // {
    //     //Debug.Log("we fired!!");
    //     float posX = transform.position.x;
    //     Vector3 bulletPosition = (transform.position);
    //     Instantiate(ProjectileItem, transform.position, transform.rotation);
    // }
    private void updateTimers()
    {
        attackTimer += Time.deltaTime;
    }

    public Vector3Int getPosition()
    {
        return Vector3Int.FloorToInt(transform.position);
    }

}