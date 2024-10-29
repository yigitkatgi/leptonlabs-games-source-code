using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberController : WeaponController
{
    public Vector3 offset;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedLightsaber = Instantiate(weaponData.Prefab);
        spawnedLightsaber.transform.parent = transform;
        spawnedLightsaber.transform.position = transform.position + offset;
    }
}
