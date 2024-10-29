using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

/// <summary>
/// Base script of all projectile weapons (to be inherited by a script which will be placed on a prefab of a projectile weapon)
/// </summary>
public class ProjectileWeaponBehavior : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    protected Vector3 direction; // Track the direction the weapon should face
    public float destroyAfterSeconds;

    // Current stats
    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
        currentPierce = weaponData.Pierce;
    }

    public float GetCurrentDamage()
    {
        return currentDamage *= FindObjectOfType<PlayerStats>().CurrentMight;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds); // Destroy the projectile after destroyAfterSeconds seconds
    }

    public void DirectionChecker(Vector3 dir)
    {
        direction = dir; // Get the argument (commonly pm.movementDirection) as variable

        // Get the argument's x and y components for the upcoming if block
        float dirx = direction.x; 
        float diry = direction.y;

        // Create copies of localScale and rotation to change and reapply to the transform
        Vector3 scale = transform.localScale; 
        Vector3 rotation = transform.rotation.eulerAngles;

        // If block to make the knife face the direction of movement (for all 8 directions) (Right already works by default since we set rotation of z to -45 deg)
        if (dirx < 0 && diry == 0) // Left
        {
            scale.x = -scale.x;
            scale.y = -scale.y; 

        }
        else if (dirx == 0 && diry < 0) // Down
        {
            scale.y *= -scale.y;
        }
        else if (dirx == 0 && diry > 0) // Up
        {
            scale.x *= -scale.x;
        }
        else if (dirx > 0 && diry > 0) // Right Up
        {
            rotation.z = 0;
        }
        else if (dirx > 0 && diry < 0)  // Right Down
        {
            rotation.z = 2 * rotation.z;
        }
        else if (dirx < 0 && diry > 0) // Left Up
        {
            rotation.z = -2 * rotation.z;
        }
        else if (dirx < 0 && diry < 0) // Left Down
        {
            rotation.z = 4 * rotation.z;
        }


        transform.localScale = scale; 
        transform.rotation = Quaternion.Euler(rotation);
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        // Reference the script from the collided collider and deal damage using TakeDamage()
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position);
            ReducePierce();
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
                ReducePierce();
            }
        }
    }

    void ReducePierce()
    {
        currentPierce--;
        if (currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}
