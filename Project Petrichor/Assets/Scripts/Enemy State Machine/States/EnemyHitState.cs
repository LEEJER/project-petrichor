using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : EnemyState
{
    private float invincibilityDuration = 0.5f;
    private float time                  = 0f;
    private NextState nextState         = NextState.Nothing;

    public override void EnterState(EnemyStateMachine enemy)
    {
        nextState = NextState.Nothing;
        if (enemy.Health <= 0)
        {
            nextState = NextState.Die;
            enemy.SwitchState(enemy.DieState);
        }
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

    }
}
