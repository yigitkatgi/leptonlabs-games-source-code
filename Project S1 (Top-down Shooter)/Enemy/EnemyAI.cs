using UnityEngine;
using Pathfinding;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public Transform target; // The player
    public float speed = 3f; // Movement speed
    public float nextWaypointDistance = 1f; // Distance to the next waypoint before moving to the next
    public float detectionRange = 10f; // Detection range for the enemy
    public LayerMask layerMask; // Layer mask with both the player and the walls
    public float shootingCooldown = 2f; // Cooldown time between shots
    public float turnSpeed = 3f;

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    private bool playerDetected = false;
    private float timer;
    private bool hasSight;
    public bool isWalking = false;
    public bool isDead = false;
    private EnemyShooting enemyShooting;
    private Seeker seeker;
    private Rigidbody2D rb;
    private EnemyManager enemyManager;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<PlayerMovementScript>().gameObject.transform;
        enemyShooting = GetComponent<EnemyShooting>();
        enemyManager = FindObjectOfType<EnemyManager>();

        timer = 0f;
        isWalking = false;
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines(); // Ensure all pathfinding updates stop when dead
            return;
        }

        timer += Time.deltaTime;

        if (!playerDetected)
        {
            DetectPlayer();
        }
        CheckLineOfSight();

        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        if (hasSight)
        {
            FacePlayer();
            rb.velocity = Vector2.zero;
            isWalking = false; // Update walking state

            if (timer >= shootingCooldown)
            {
                enemyShooting.Shoot();
                timer = 0f; // Reset the timer after shooting
            }
        }
        else
        {
            // Follow the path
            FollowPath();
            isWalking = true; // Update walking state
        }
    }

    void DetectPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.position - transform.position).normalized, detectionRange, layerMask);

        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerDetected = true;
            enemyManager.OnEnemyDetectsPlayer(this); // Notify the manager
            StartPathfinding(); // Start pathfinding when player is detected
        }
    }

    void CheckLineOfSight()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, layerMask);

        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            hasSight = true;
        }
        else
        {
            hasSight = false;
        }
    }

    void StartPathfinding()
    {
        if (!IsInvoking("UpdatePath"))
        {
            InvokeRepeating("UpdatePath", 0f, 0.5f); // Start updating the path
        }
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FollowPath()
    {
        if (path == null || isDead)
        {
            return;
        }

        // Direction to the next waypoint
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 targetPosition = (Vector2)path.vectorPath[currentWaypoint];

        // Move the enemy towards the target position
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
        rb.MovePosition(newPosition);

        // Rotate the enemy to face the direction of movement smoothly
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * Time.deltaTime);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        // Check if we reached the next waypoint
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void FacePlayer()
    {
        Vector3 faceDirection = target.position - transform.position;
        float faceAngle = Mathf.Atan2(faceDirection.y, faceDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, faceAngle - 90);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    public void KillEnemy()
    {
        isDead = true;
        CancelInvoke("UpdatePath"); // Stop pathfinding updates when enemy is killed
        path = null; // Clear the path to stop rendering lines
        rb.velocity = Vector2.zero; // Stop enemy movement
        seeker.enabled = false; // Disable the seeker to ensure no further pathfinding is triggered

        if (enemyManager != null)
        {
            enemyManager.OnEnemyKilled(this); // Notify the manager
        }

        // Optionally, you can add additional cleanup logic here
    }
}
