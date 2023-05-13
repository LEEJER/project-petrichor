using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float        damage              = 1f;

    public bool         isAttacking         = false;
    public int          num                 = 0;
    public Vector2      dir                 = Vector2.zero;
    public float        swingMovementSpeed  = 1.45f;
    public float        knockbackForce      = 2.5f;
    public float        hitboxOffset        = 0.12f;

    private Animator    animator;
    private Collider2D  swordHitBox;
    private Transform   player;
    private PlayerStateMachine playerScript;

    public void Start()
    {
        animator            = GetComponent<Animator>();
        swordHitBox         = GetComponent<Collider2D>();
        swordHitBox.enabled = false;
        swordHitBox.offset  = Vector2.zero;
        player              = transform.parent;
        playerScript        = player.GetComponent<PlayerStateMachine>();
    }

    public void DisableSwordHitbox()
    {
        swordHitBox.enabled = false;
    }

    public void EnableSwordHitbox()
    {
        swordHitBox.enabled = true;
    }

    public void SwordAttack(Vector2 direction, int number)
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        dir = direction;
        swordHitBox.enabled = true;
        float sign_x = Mathf.Sign(dir.x);
        float sign_y = Mathf.Sign(dir.y);

        // in the horizontal direction

        if (Mathf.Abs(dir.x) * 2 > Mathf.Abs(dir.y))
        {
            // you really dont want to see the monster that this following line used to be before I figured out how to simplifiy it
            transform.Rotate(Vector3.forward, sign_x * sign_y * Vector2.Angle(Vector2.right * sign_x, dir) * 0.4f);
            swordHitBox.offset = new Vector2(Mathf.Sign(dir.x) * hitboxOffset, 0);
        } 
        // in the vertical direction
        else
        {
            // you really dont want to see the monster that this following line used to be before I figured out how to simplifiy it
            transform.Rotate(Vector3.forward, sign_x * sign_y * Vector2.Angle(Vector2.up * sign_y, dir) * -0.8f);
            swordHitBox.offset = new Vector2(0, Mathf.Sign(dir.y) * hitboxOffset);
        }

        animator.SetFloat("dirX", dir.x);
        animator.SetFloat("dirY", dir.y);
        animator.SetInteger("attackNum", number);
        animator.SetTrigger("attack");
    }
}
