using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
public class PlayerController : MonoBehaviour
{
    private FPMelee inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;

    // For camera
    [Header("Camera Settings")]
    public Transform playerCamera;
    public Transform cameraParentTransform;
    public float sensitivity = 2f;
    public float minVerticalAngle = -80f; // Define el ángulo mínimo de rotación vertical
    public float maxVerticalAngle = 80f; // Define el ángulo máximo de rotación vertical

    // For movement
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    // For jump
    [Header("Jump Settings")]
    public float jumpForce = 500f;
    private bool isGrounded = true;

    // For attack
    [Header("Attack Settings")]
    public float attackSpeed = 2f;
    public float attackSpeed4 = 2f;
    public int numberOfAttackStages = 4;
    public float attackDelay = 0.4f;
    private bool readyToAttack = true;
    private bool isAttacking = false;
    private int attackStage = 0;
    private float attackHoldTime = 0f;
    private bool isAttackingHold = false;
    public float tiempoUmbral = 2.0f;

    private bool isImmune = false;
    public float immuneDuration = 1.5f; // Duración de la inmunidad en segundos

    // Animation states
    Animator animator;
    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string ATTACK1 = "Attack01";
    public const string ATTACK2 = "Attack02";
    public const string ATTACK3 = "Attack03";
    public const string JUMP = "Jump";

    private Rigidbody rb;

    public event Action OnPlayerDamaged;

    private void Awake()
    {
        inputActions = new FPMelee();
        rb = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator is null!");
            return;
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Look.performed += OnLookInput;
        inputActions.Player.Attack.performed += OnAttackInput;
        inputActions.Player.Attack.canceled += OnAttackInputEnded;
        inputActions.Player.Jump.performed += OnJumpInput;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Look.performed -= OnLookInput;
        inputActions.Player.Attack.performed -= OnAttackInput;
        inputActions.Player.Attack.canceled -= OnAttackInputEnded;
        inputActions.Player.Jump.performed -= OnJumpInput;
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();

        Vector3 currentRotation = playerCamera.localRotation.eulerAngles;
        //Vector3 currentRotation = cameraParentTransform.localRotation.eulerAngles;


        float newRotationX = currentRotation.x - lookInput.y * sensitivity;
        newRotationX = ClampAngle(newRotationX, minVerticalAngle, maxVerticalAngle);

        transform.Rotate(Vector3.up, lookInput.x * sensitivity);

        playerCamera.localRotation = Quaternion.Euler(newRotationX, currentRotation.y, 0f);
        //cameraParentTransform.localRotation = Quaternion.Euler(newRotationX, currentRotation.y, 0f);

    }
    public bool IsPlayerAttacking()
    {
        return isAttacking;
    }
    private void OnJumpInput(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            isGrounded = false;
            Debug.Log("Salta");
            //rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Aplicar fuerza de salto hacia arriba
            ChangeAnimationState(JUMP); // Cambiar a la animación de salto
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        return Mathf.Clamp((angle <= 180) ? angle : -(360 - angle), min, max);
    }

    private void OnAttackInput(InputAction.CallbackContext context)
    {
        if (!readyToAttack || isAttacking)
        {
            return;
        }

        readyToAttack = false;
        isAttacking = true;

        attackHoldTime = Time.time;
        isAttackingHold = true;

    }
    private void OnAttackInputEnded(InputAction.CallbackContext context)
    {
        if (isAttackingHold)
        {
            isAttackingHold = false;
            isAttacking = true;

            // Calcular el tiempo transcurrido
            float holdDuration = Time.time - attackHoldTime;
            Debug.Log(holdDuration);
            // Si el tiempo transcurrido supera cierto umbral, ejecutar el ataque 4
            
            Invoke(nameof(ResetAttack), attackSpeed);
            if (attackStage == 0)
            {
                Debug.Log("ataque 1");
                ChangeAnimationState(ATTACK1);
                attackStage++;
            }
            else if (attackStage == 1)
            {
                ChangeAnimationState(ATTACK2);
                attackStage++;
            }
            else
            {
                ChangeAnimationState(ATTACK3);
                attackStage = 0;
            }

            attackHoldTime = 0f;
        }
    }
    void ResetAttack()
    {
        isAttacking = false;
        readyToAttack = true;
    }
    private void FixedUpdate()
    {
        // Movimiento
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        // Si no se están presionando las teclas de movimiento, detener al personaje
        if (!isAttacking && isGrounded || isAttackingHold)
        {
            if (moveInput.magnitude < 0.1f)
            {
                ChangeAnimationState(IDLE);
            }
            else
            {
                ChangeAnimationState(WALK);
            }
        }

        // Mover al personaje directamente sin usar fuerzas
        transform.position += moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;
    }

    string currentAnimationState;
    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // El jugador está en el suelo
        }
        else if (collision.gameObject.CompareTag("Enemy") && !isImmune) // Asegúrate de que el jugador no esté inmune
        {
            Debug.Log("Me ha golpeado el enemigo");
            GameManager.instance.LoseLife();
            
            // Activar la inmunidad y programar la desactivación después de un cierto tiempo
            StartCoroutine(ActivateImmunity());

            OnPlayerDamaged?.Invoke();
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Traps") && !isImmune) // Asegúrate de que el jugador no esté inmune
        {
            Debug.Log("Me han dañado una trampa");
            GameManager.instance.LoseLife();
            
            // Activar la inmunidad y programar la desactivación después de un cierto tiempo
            StartCoroutine(ActivateImmunity());

            OnPlayerDamaged?.Invoke();
        }
        else if (other.CompareTag("EndPoint"))
        {
            GameManager.instance.SetReachedEndPoint(); // Notificar al GameManager que el jugador ha llegado al punto final
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //isGrounded = false; // El jugador ya no está en el suelo
        }
    }
    public bool IsAttacking()
    {
        return isAttacking;
    }
    private IEnumerator ActivateImmunity()
    {
        isImmune = true;
        
        yield return new WaitForSeconds(immuneDuration);
        
        isImmune = false;
    }

}

