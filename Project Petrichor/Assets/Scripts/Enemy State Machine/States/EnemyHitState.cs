using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : EnemyState
{
    private float duration = 0.33f;
    private float time                  = 0f;
    private NextState nextState         = NextState.Nothing;

    public override void EnterState(EnemyStateMachine enemy)
    {
        enemy.currentState = EnemyStateMachine.CurrentState.Hit;
        nextState = NextState.Nothing;
        time = 0f;
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        if (enemy.Health <= 0)
        {
            nextState = NextState.Die;
        }
        switch (nextState)
        {
            case NextState.Die:
                enemy.SwitchState(enemy.DieState);
                break;
            case NextState.Idle:
                enemy.SwitchState(enemy.IdleState);
                break;
            default:
                break;
        }
        time += Time.fixedDeltaTime;
        if (time >= duration)
        {
            nextState = NextState.Idle;
        }
    }

    public override void ExitState(EnemyStateMachine enemy)
    {
        nextState = NextState.Nothing;
        time = 0f;
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
                // take knockback
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