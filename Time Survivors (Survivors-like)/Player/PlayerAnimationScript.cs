using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    Animator animator;
    PlayerMovementScript playerMovementScript;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovementScript = GetComponent<PlayerMovementScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovementScript.moveDirection.x != 0 || playerMovementScript.moveDirection.y != 0)
        {
            animator.SetBool("Move", true);
            SpriteDirectionChecker();
        } else
        {
            animator.SetBool("Move", false);
        }
    }

    void SpriteDirectionChecker()
    {
        if (playerMovementScript.lastHorizMoveVector < 0)
        {
            spriteRenderer.flipX = true;
        } else
        {
            spriteRenderer.flipX = false;
        }

    }

    public void SetAnimatorController(RuntimeAnimatorController controller)
    {
        if (!animator) animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = controller;
    }
}
