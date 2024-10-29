using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarlicBehavior : MeleeWeaponBehavior
{
    List<GameObject> markedEnemies;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && !markedEnemies.Contains(col.gameObject))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position, 0, 0);

            markedEnemies.Add(col.gameObject); // Mark the damaged enemy so it won't be damaged by the same garlic again
        }
        else if (col.CompareTag("Prop") && !markedEnemies.Contains(col.gameObject))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());

                markedEnemies.Add(col.gameObject); // Treat breakable props as enemies so we do not damage them continuously continuously
            }
        }
    }

}
