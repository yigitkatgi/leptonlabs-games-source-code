using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaCoilController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject zap = Instantiate(weaponData.Prefab);
        zap.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
        zap.transform.position = transform.position; // Set the pos. of the knife the same as this object who is a child of the player object (same pos)
        zap.transform.SetParent(null);
        Debug.Log("Instantiated Zap Scale: " + zap.transform.localScale);
    }

}
