using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private Animator animator;
    private Vector2 attackDirection;
    private int attackNum = 0;
    private bool isAttacking = false;
    //private bool attackQueued = false;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        attackDirection = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SwordAnimate();
    }

    private void SwordAnimate()
    {
        animator.SetFloat("dirX", attackDirection.x);
        animator.SetFloat("dirY", attackDirection.y);
        animator.SetInteger("attackNum", attackNum);
        animator.SetBool("isAttacking", isAttacking);
        //animator.SetBool("attackQueued", attackQueued);
    }

    public void AttackWithSword(Vector2 direction, int attackNum, bool isAttacking)
    {
        this.attackDirection = direction;
        this.attackNum = attackNum;
        this.isAttacking = isAttacking;
        //this.attackQueued = attackQueued;
        SwordAnimate();
    }

    private void EventEndAttackWithSword()
    {
        isAttacking = false;
    }
}
