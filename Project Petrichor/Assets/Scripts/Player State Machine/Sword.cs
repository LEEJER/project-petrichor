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
    public float        hitboxOffset        = 0.15f;

    private Animator animator;
    private Collider2D swordHitBox;
    private Transform player;
    private PlayerStateMachine playerScript;

    public void Start()
    {
        animator = GetComponent<Animator>();
        swordHitBox = GetComponent<Collider2D>();
        swordHitBox.enabled = false;
        swordHitBox.offset = Vector2.zero;
        player = transform.parent;
        playerScript = player.GetComponent<PlayerStateMachine>();
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
        dir = direction;
        swordHitBox.enabled = true;
        if (Mathf.Abs(direction.x) * 2 > Mathf.Abs(direction.y))
        {
            swordHitBox.offset = new Vector2(Mathf.Sign(direction.x) * hitboxOffset, 0);
        } 
        else
        {
            swordHitBox.offset = new Vector2(0, Mathf.Sign(direction.y) * hitboxOffset);
        }

        animator.SetFloat("dirX", direction.x);
        animator.SetFloat("dirY", direction.y);
        animator.SetInteger("attackNum", number);
        animator.SetTrigger("attack");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            if (enemy != null)
            {
                EnemyStateMachine enemyScript = enemy.GetComponent<EnemyStateMachine>();
                bool hit = enemyScript.TakeDamage(damage, dir.normalized * knockbackForce);
                if (hit)
                {
                    playerScript.VelocityVector += -1f * dir.normalized * (knockbackForce/1.5f);
                }
            }
        } 
    }
}
