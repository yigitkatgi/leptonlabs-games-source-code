using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatScript : MonoBehaviour
{
    public bool isAttacking = false;

    private PlayerMovementScript pm;

    private void Start()
    {
        pm = GetComponent<PlayerMovementScript>();
    }

    private void Update()
    {
        if (pm.isMoving)
        {

        }
        else
        {

        }
    }

    private void Attack()
    {

    }
}
