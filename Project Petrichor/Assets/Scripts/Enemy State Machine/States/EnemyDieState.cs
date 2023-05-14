using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieState : EnemyState
{
    public override void EnterState(EnemyStateMachine enemy)
    {
        enemy.currentState = EnemyStateMachine.CurrentState.Die;
        //enemy.animator.SetTrigger("t_die");
        EventEndDeathAnimation(enemy);
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {

    }

    public override void ExitState(EnemyStateMachine enemy)
    {

    }

    public void EventEndDeathAnimation(EnemyStateMachine enemy)
    {
        enemy.RemoveSelf();
    }

    public override void OnHitboxEnter(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {
        GameObject other = collision.gameObject;
        if (selfComponent == "Hitbox")
        {
            // if we are hit by the player hurtbox
            if (other.layer == LayerMask.NameToLayer("Player") && other.CompareTag("Hurtbox"))
            {
                Sword sword = other.GetComponent<Sword>();
                // dont take damage
                // dont go to hit state
                // get pushed
                enemy.VelocityVector += sword.dir.normalized * sword.knockbackForce * enemy.KnockbackResistance;
            }
        }
        
    }

    public override void OnHitboxStay(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {

    }
    public override void OnHitboxExit(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {

    }
}
