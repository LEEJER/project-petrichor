using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public void AttackWithSword(Vector2 direction, int attackNum)
    {
        animator.SetFloat("dirX", direction.x);
        animator.SetFloat("dirY", direction.y);
        animator.SetInteger("attackNum", attackNum);
        animator.SetTrigger("attack");
    }

    public void EventEndAttackWithSword()
    {

    }
}
