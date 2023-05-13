using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : EnemyState
{
    private float invincibilityDuration = 3f;
    private float time                  = 0f;
    private NextState nextState         = NextState.Nothing;

    public override void EnterState(EnemyStateMachine enemy)
    {
        enemy.currentState = EnemyStateMachine.CurrentState.Hit;
        nextState = NextState.Nothing;
        if (enemy.Health <= 0)
        {
            nextState = NextState.Die;
            enemy.SwitchState(enemy.DieState);
        }
        time = 0f;
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        switch (nextState)
        {
            case NextState.Chase:
                enemy.SwitchState(enemy.ChaseState);
                break;
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
        if (time >= invincibilityDuration)
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

    }

    public override void OnHitboxStay(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {

    }

    public override void OnHitboxExit(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {

    }
}
