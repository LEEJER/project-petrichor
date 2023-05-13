using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    NextState nextState = NextState.Nothing;
    public override void EnterState(EnemyStateMachine enemy)
    {
        nextState = NextState.Nothing;
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        switch (nextState)
        {
            case NextState.Idle:
                enemy.SwitchState(enemy.IdleState);
                break;
            case NextState.Chase:
                enemy.SwitchState(enemy.ChaseState);
                break;
            case NextState.Hit:
                enemy.SwitchState(enemy.HitState);
                break;
            default:
                break;
        }
    }

    public override void ExitState(EnemyStateMachine enemy)
    {
        nextState = NextState.Nothing;
    }

    public override void OnDetectionBoxEnter(EnemyStateMachine enemy, Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            nextState = NextState.Chase;
        }
    }

    public override void OnDetectionBoxStay(EnemyStateMachine enemy, Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            nextState = NextState.Chase;
        }
    }
    public override void OnDetectionBoxExit(EnemyStateMachine enemy, Collider2D collision)
    {

    }

    public override void OnTakeDamage(EnemyStateMachine enemy, float damage, Vector2 push)
    {
        enemy.Health -= damage;
        enemy.VelocityVector += push;
        nextState = NextState.Hit;
    }
}
