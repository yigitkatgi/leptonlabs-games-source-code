using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncendiaryControllerScript : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedGrenade = Instantiate(weaponData.Prefab);
        spawnedGrenade.transform.position = transform.position;
        spawnedGrenade.GetComponent<ProjectileWeaponBehavior>().DirectionChecker(pm.lastMovedVector);
    }
}
