using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public GameObject bloodPrefab;
    public GameObject pistolPrefab;
    public GameObject shotgunPrefab;
    public GameObject carbinePrefab;
    public float offset = 0.5f;

    private Transform bloodPosition;
    private Vector2 direction;
    private Collider2D firingEntityCollider;

    public void Initialize(Vector2 initialDirection, Collider2D firingEntity)
    {
        direction = initialDirection.normalized;
        firingEntityCollider = firingEntity;

        // Ignore collision with the firing entity initially
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), firingEntityCollider, true);
        //StartCoroutine(EnableCollision()); 
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // Damage the player
        }
        else if (col.gameObject.CompareTag("Enemy") && !firingEntityCollider.gameObject.CompareTag("Enemy"))
        {
            EnemyAI enemyAI = col.gameObject.GetComponent<EnemyAI>();
            if (enemyAI != null && !enemyAI.isDead)
            {
                // Set the enemy as dead to prevent multiple drops
                enemyAI.isDead = true;

                // Disable enemy collider, enemyAI, and shooting script
                col.gameObject.GetComponent<Collider2D>().enabled = false;
                enemyAI.KillEnemy();
                col.gameObject.GetComponent<EnemyAI>().enabled = false;
                col.gameObject.GetComponent<EnemyShooting>().enabled = false;

                // Remove seeker component
                var seeker = col.gameObject.GetComponent<Seeker>(); 
                if (seeker != null)
                {
                    Destroy(seeker); // Remove the Seeker component
                }

                // Test
                //Destroy(col.gameObject);

                // Play the blood effect
                bloodPosition = col.gameObject.transform.Find("Blood");
                GameObject bloodFX = Instantiate(bloodPrefab, bloodPosition.position, bloodPrefab.transform.rotation);

                ParticleSystem blood = bloodFX.GetComponent<ParticleSystem>();

                if (blood != null)
                {
                    var shape = blood.shape;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    shape.rotation = new Vector3(0, 0, angle + 65 - 90);
                }

                // Play dead animation
                EnemyAnimator anim = col.gameObject.GetComponent<EnemyAnimator>();
                anim.PlayDeadAnimation();

                // Drop the weapon
                DropWeapon(col.gameObject);
            }
        }
        else if (col.gameObject.CompareTag("Enemy") && firingEntityCollider.gameObject.CompareTag("Enemy"))
        {
            // Avoid friendly fire of enemies
            Collider2D hitEnemyCollider = col.gameObject.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hitEnemyCollider);
            return;
        }
        else if (col.gameObject.CompareTag("Bullet"))
        {
            // Let bullets pass through each other
            Collider2D bulletCollider = col.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bulletCollider);
            return;
        }

        // Destroy the bullet after hitting something
        Destroy(gameObject);
    }

    void DropWeapon(GameObject enemy)
    {
        Pistol pistol = enemy.GetComponentInChildren<Pistol>();
        Shotgun shotgun = enemy.GetComponentInChildren<Shotgun>();
        Carbine carbine = enemy.GetComponentInChildren<Carbine>();

        if (pistol != null)
        {
            Debug.Log("Spawning pistol");
            Instantiate(pistolPrefab, enemy.transform.position + offset * new Vector3(Mathf.Cos(Random.Range(0, 2 * Mathf.PI)), Mathf.Sin(Random.Range(0, 2 * Mathf.PI)), 0), Quaternion.identity);
        }
        if (shotgun != null)
        {
            Debug.Log("Spawning shotgun");
            Instantiate(shotgunPrefab, enemy.transform.position + offset * Vector3.up, Quaternion.identity);
        }
        if (carbine != null)
        {
            Debug.Log("Spawning carbine");
            Instantiate(carbinePrefab, enemy.transform.position + offset * Vector3.up, Quaternion.identity);
        }
    }

    IEnumerator EnableCollision()
    {
        // Wait for a short duration before enabling collision
        yield return new WaitForSeconds(0.1f);
        if (firingEntityCollider != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), firingEntityCollider, false);
        }
    }
}
