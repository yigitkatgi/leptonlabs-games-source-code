using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponScriptableObject weaponData;
    protected float currentCooldown;

    protected PlayerMovementScript pm; // Reference pm so children have access (used to get the player mov. direction to set it as projectile dir.)

    // Start is called before the first frame update
    protected virtual void Start()
    {
        pm = FindObjectOfType<PlayerMovementScript>();
        currentCooldown = weaponData.CooldownDuration; // So that the weapon does not fire immediately and has to wait for cd
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;

        if (currentCooldown <= 0f) // Attack when cooldown is zero
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        currentCooldown = weaponData.CooldownDuration;
    }
}
