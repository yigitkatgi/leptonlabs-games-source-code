using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : WeaponController
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedKnife = Instantiate(weaponData.Prefab);
        spawnedKnife.transform.position = transform.position; // Set the pos. of the knife the same as this object who is a child of the player object (same pos)
        spawnedKnife.GetComponent<KnifeBehavior>().DirectionChecker(pm.lastMovedVector); // Reference and set the direction (same as player mov. direction)
    }
}
