using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float rollForce = 40f;
    [SerializeField] private float jumpForce = 5f;
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded;
    private bool isRolling;
    
    // Events
    public event Action OnCubeRoll;
    public event Action OnCubeJump;
    public event Action<Vector3> OnCubeMove;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 50f; // Allow for faster rotation
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.Playing) return;

        HandleInput();
        CheckGround();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.Playing) return;

        Move();
        Roll();
    }

    private void HandleInput()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Calculate move direction relative to camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;

            forward.y = 0;
            right.y = 0;
            
            forward.Normalize();
            right.Normalize();

            moveDirection = (forward * vertical + right * horizontal).normalized;
        }
        else
        {
            moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        }

        // Handle jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void Move()
    {
        if (moveDirection != Vector3.zero && !isRolling)
        {
            // Apply movement force
            rb.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
            
            // Rotate the cube to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            OnCubeMove?.Invoke(moveDirection);
        }
    }

    private void Roll()
    {
        if (moveDirection != Vector3.zero && isGrounded)
        {
            // Apply rolling torque
            Vector3 rollAxis = Vector3.Cross(Vector3.up, moveDirection);
            rb.AddTorque(rollAxis * rollForce, ForceMode.Acceleration);
            
            if (!isRolling)
            {
                isRolling = true;
                OnCubeRoll?.Invoke();
            }
        }
        else
        {
            isRolling = false;
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        OnCubeJump?.Invoke();
    }

    private void CheckGround()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );
    }

    // Add force from external sources (e.g., powerups, collisions)
    public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Impulse)
    {
        rb.AddForce(force, forceMode);
    }

    // Stop all movement (e.g., when game is paused)
    public void StopMovement()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw ground check ray
        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.down * groundCheckDistance
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision effects
        if (collision.relativeVelocity.magnitude > 10f)
        {
            // TODO: Spawn impact effects
            // TODO: Play impact sound
        }
    }
}