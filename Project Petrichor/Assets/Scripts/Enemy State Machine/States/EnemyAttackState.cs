using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public override void EnterState(EnemyStateMachine enemy)
    {

    }

    public override void UpdateState(EnemyStateMachine enemy)
    {

    }

    public override void ExitState(EnemyStateMachine enemy)
    {

    }

    public override void OnDetectionBoxEnter(EnemyStateMachine enemy, Collider2D collision)
    {

    }

    public override void OnDetectionBoxStay(EnemyStateMachine enemy, Collider2D collision)
    {

    }
    public override void OnDetectionBoxExit(EnemyStateMachine enemy, Collider2D collision)
    {

    }

    public override void OnTakeDamage(EnemyStateMachine enemy, float damage, Vector2 push)
    {
        enemy.Health -= damage;
        enemy.VelocityVector += push;
        enemy.SwitchState(enemy.HitState);
    }
}
