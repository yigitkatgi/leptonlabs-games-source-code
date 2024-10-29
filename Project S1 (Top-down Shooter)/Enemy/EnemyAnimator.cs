using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;
    private EnemyAI enemyAI;

    private void Start()
    {
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
    }

    private void Update()
    {
        animator.SetBool("isWalking", enemyAI.isWalking);
    }

    public void PlayDeadAnimation()
    {
        animator.SetTrigger("Die");
    }
}
