using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBehavior : MonoBehaviour, PoolManager.IPooledObject
{
    [System.Serializable]
    public class EnemyStats
    {
        public float maxHealth = 100f;
        public float moveSpeed = 5f;
        public float attackDamage = 10f;
        public float attackRange = 1.5f;
        public float attackCooldown = 1f;
    }

    [SerializeField] private EnemyStats baseStats;
    [SerializeField] private EnemyType enemyType;
    
    public enum EnemyType
    {
        Normal,
        Speedy,
        Tank,
        Ranged
    }

    private Transform target;
    private Rigidbody rb;
    private float currentHealth;
    private bool canAttack = true;
    private Vector3 lastKnownPlayerPosition;
    private float difficultyMultiplier = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Transform playerTransform)
    {
        target = playerTransform;
        difficultyMultiplier = GameManager.Instance.GetCurrentDifficulty();
        
        // Apply difficulty scaling to stats
        currentHealth = baseStats.maxHealth * difficultyMultiplier;
        
        StartCoroutine(UpdateTargetPosition());
    }

    public void OnObjectSpawn()
    {
        canAttack = true;
        currentHealth = baseStats.maxHealth * difficultyMultiplier;
        
        // Reset any effects or visual changes
        ResetEnemy();
    }

    private void ResetEnemy()
    {
        // Reset any visual effects or state changes here
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        // Movement behavior based on enemy type
        switch (enemyType)
        {
            case EnemyType.Normal:
                NormalMovement();
                break;
            case EnemyType.Speedy:
                SpeedyMovement();
                break;
            case EnemyType.Tank:
                TankMovement();
                break;
            case EnemyType.Ranged:
                RangedMovement();
                break;
        }
    }

    private void NormalMovement()
    {
        MoveTowardsTarget(baseStats.moveSpeed);
    }

    private void SpeedyMovement()
    {
        MoveTowardsTarget(baseStats.moveSpeed * 1.5f);
    }

    private void TankMovement()
    {
        MoveTowardsTarget(baseStats.moveSpeed * 0.7f);
    }

    private void RangedMovement()
    {
        // Keep distance from player
        float distanceToTarget = Vector3.Distance(transform.position, lastKnownPlayerPosition);
        if (distanceToTarget < baseStats.attackRange * 0.8f)
        {
            // Move away from player
            Vector3 directionFromTarget = (transform.position - lastKnownPlayerPosition).normalized;
            rb.MovePosition(transform.position + directionFromTarget * baseStats.moveSpeed * Time.fixedDeltaTime);
        }
        else if (distanceToTarget > baseStats.attackRange * 1.2f)
        {
            // Move towards player
            MoveTowardsTarget(baseStats.moveSpeed * 0.5f);
        }
    }

    private void MoveTowardsTarget(float speed)
    {
        Vector3 direction = (lastKnownPlayerPosition - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
        
        // Rotate towards movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.fixedDeltaTime));
        }
    }

    private IEnumerator UpdateTargetPosition()
    {
        while (gameObject.activeInHierarchy)
        {
            if (target != null)
            {
                lastKnownPlayerPosition = target.position;
                CheckAttackRange();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CheckAttackRange()
    {
        if (!canAttack) return;

        float distanceToTarget = Vector3.Distance(transform.position, lastKnownPlayerPosition);
        if (distanceToTarget <= baseStats.attackRange)
        {
            Attack();
        }
    }

    private void Attack()
    {
        canAttack = false;
        
        // Apply damage to player
        // TODO: Implement player damage system
        
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(baseStats.attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.AddKill();
        
        // Spawn death effects here
        
        // Return to pool
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && canAttack)
        {
            Attack();
        }
    }
}