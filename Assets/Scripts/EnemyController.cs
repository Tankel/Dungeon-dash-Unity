using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    private Animator animator;
    private Transform player; // Referencia al jugador

    public float attackRange = 1f; // Rango de ataque del enemigo
    private float movementSpeed = 1.5f; // Velocidad de movimiento del enemigo
    private float rotationSpeed = 4f; // Velocidad de rotación del enemigo
    private float health = 60f;
    private bool isDead = false;
    private bool isImmune = false; // Variable para controlar la inmunidad después de ser golpeado
    public float knockbackForce = 5f;
    public float raycastDist = 2f;
    private Rigidbody rb;
    private SphereCollider sphereCollider;
    private bool wasAttacked = false;
    public ParticleSystem blood;
    FieldOfView fieldOfView;

    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string RUN = "Run";
    public const string ATTACK = "Attack";
    public const string GETHIT = "GetHit";
    public const string DIE = "Die";

    public float immunityDuration = 0.2f; // Duración de la inmunidad después de ser golpeado
    string currentAnimationState;

    public int rutina;
    public float cronometro;
    public Quaternion angulo;
    public float grado;


    private void Start()
    {
        fieldOfView = GetComponent<FieldOfView>();
        blood.Stop();
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //visionCone = GetComponentInChildren<VisionCone>(); // Obtener la referencia al componente VisionCone
        ChangeAnimationState(IDLE);
    }

    private void Update()
    {
        if (wasAttacked)
        {
            if (!isImmune && !isDead) // Si no está inmune, seguirá moviéndose
            {
                // SOLO DURANTE UN TIEMPO, SI NO LO ENCUENTRA SE VULEVE FALSO.
                if (PlayerInAttackRange())
                {
                    AttackPlayer();
                }
                else
                {
                    MoveTowardsPlayer();
                }
            }
        }
        else
        {
            if ((!isDead && !isImmune) ) // Si no está muerto ni inmune, seguirá moviéndose
            {
                if (PlayerInDetectionRange())
                {
                    if (PlayerInAttackRange())
                    {
                        AttackPlayer();
                    }
                    else
                    {
                        MoveTowardsPlayer();
                    }
                }
                else
                {
                    //Debug.Log("patrullaraaa");
                    Patrol();
                }
            }
        }
    }

    private bool PlayerInDetectionRange()
    {
        return fieldOfView.canSeePlayer;
    }

    private bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange;
    }

    private void MoveTowardsPlayer()
    {
        wasAttacked = true;
        // Rotar hacia el jugador
        Vector3 direction = player.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        // Mover hacia el jugador
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);

        ChangeAnimationState(RUN);
    }

    private void AttackPlayer()
    {
        ChangeAnimationState(ATTACK);
        // Lógica de ataque al jugador
    }

    private void Patrol()
    {
        /*
        cronometro += 1 * Time.deltaTime;
        if (cronometro >= 4)
        {
            rutina = Random.Range(0, 2);
            cronometro = 0;
        }

        switch (rutina)
        {
            case 0:
                ChangeAnimationState(IDLE);
                break;
            case 1:
                grado = Random.Range(0, 360);
                angulo = Quaternion.Euler(0, grado, 0);
                rutina++;
                break;
            case 2:
                transform.rotation = Quaternion.RotateTowards(transform.rotation,angulo, 0.5f);
                transform.Translate(Vector3.forward * 1 * Time.deltaTime);
                ChangeAnimationState(WALK);
                break;
        }*/
        
        // Generar una posición aleatoria en el plano XY dentro de un rango determinado
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        Vector3 targetPosition = transform.position + randomDirection * Random.Range(5f, 10f); // Rango de movimiento aleatorio

        // Utilizar un raycast desde la posición actual del enemigo hacia la posición generada
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetPosition - transform.position, out hit, raycastDist, fieldOfView.obstructionMask))
        {
            // Si el raycast colisiona con un objeto en las capas especificadas (Walls o Obstructions), rotar al enemigo hacia una nueva dirección
            Vector3 newDirection = Vector3.Reflect(randomDirection, hit.normal);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        // Mover al enemigo hacia la nueva dirección
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
        ChangeAnimationState(WALK);
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Weapon")  && player.GetComponent<PlayerController>().IsPlayerAttacking())
        {
            if (!isImmune) // Si no está inmune, tomar daño
            {
                wasAttacked = true;
                blood.Play();
                TakeDamage(20); // Otra opción sería pasar el daño del arma como parámetro
                StartCoroutine(ImmunityCooldown()); // Iniciar el tiempo de inmunidad
            }
        }
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            //Vector3 knockbackDirection = (transform.position - player.position).normalized;
            //rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

            ChangeAnimationState(GETHIT);
        }
    }

    private void Die()
    {
        isDead = true;
        sphereCollider.enabled = false;
        ChangeAnimationState(DIE);
        // destruir objeto despues de cierto tiempo o aplicar alguna lógica de juego
    }

    private IEnumerator ImmunityCooldown()
    {
        isImmune = true; // Establecer inmunidad
        yield return new WaitForSeconds(immunityDuration); // Esperar la duración de la inmunidad
        isImmune = false; // Desactivar inmunidad
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }
}
