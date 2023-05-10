using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public bool         isAttacking         = false;
    public int          num                 = 0;
    public Vector2      dir                 = Vector2.zero;
    public float        swingMovementSpeed  = 1.45f;

    private Animator    animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    // returns true if an attack already exists
    public bool TryAddAttack(Vector2 direction)
    {
        bool val = (dir != Vector2.zero);
        AddAttack(direction);
        return val;
    }

    private void AddAttack(Vector2 dir)
    {
        this.dir = dir;
        num = (num + 1) % 2;
        isAttacking = true;
    }

    public void SwordAttack()
    {
        animator.SetFloat("dirX", dir.x);
        animator.SetFloat("dirY", dir.y);
        animator.SetInteger("attackNum", num);
        animator.SetTrigger("attack");
    }
}
