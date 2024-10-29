using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    private PlayerMovementScript pm;
    private Animator am;

    private void Start()
    {
        pm = GetComponent<PlayerMovementScript>();
        am = GetComponent<Animator>(); 
    }

    private void Update()
    {
        am.SetBool("isRunning", pm.isMoving); 
        am.SetBool("inAir", !pm.isGrounded);
        am.SetBool("isDashing", pm.isDashing);
    }
}
