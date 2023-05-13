using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
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
            case NextState.Attack:
                enemy.SwitchState(enemy.AttackState);
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

    public override void OnDetectionBoxEnter(EnemyStateMachine enemy, Collider2D col)
    {

    }

    public override void OnDetectionBoxStay(EnemyStateMachine enemy, Collider2D col)
    {

    }
    public override void OnDetectionBoxExit(EnemyStateMachine enemy, Collider2D col)
    {
        nextState = NextState.Patrol; 
    }

    public override void OnTakeDamage(EnemyStateMachine enemy, float damage, Vector2 push)
    {
        enemy.Health -= damage;
        enemy.VelocityVector += push;
        nextState = NextState.Hit;
    }
}
