using System.Collections;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;
    public float pushSpeed;
    public float pushDuration;
    public float destroyDistance;

    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        playerCollector.radius = player.CurrentMagnet;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            StartCoroutine(CollectWithPushAndPull(col, collectible));
        }
    }

    IEnumerator CollectWithPushAndPull(Collider2D col, ICollectible collectible)
    {
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        Collider2D colCollider = col.GetComponent<Collider2D>();

        // Disable the collectible's collider to prevent further interactions
        if (colCollider != null)
        {
            colCollider.enabled = false;
        }

        // Calculate direction away from the player and apply initial push force
        Vector2 pushDirection = (col.transform.position - player.transform.position).normalized;
        rb.AddForce(pushDirection * pushSpeed, ForceMode2D.Impulse);

        // Wait for pushDuration seconds
        yield return new WaitForSeconds(pushDuration);

        // Continuously pull towards the player
        while (Vector2.Distance(col.transform.position, player.transform.position) > destroyDistance)
        {
            Vector2 pullDirection = (player.transform.position - col.transform.position).normalized;
            rb.velocity = pullDirection * pullSpeed;
            yield return null;
        }

        // Call the Collect method
        collectible.Collect();

        // Destroy the collectible
        Destroy(col.gameObject);
    }
}
