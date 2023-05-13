using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieState : EnemyState
{
    public override void EnterState(EnemyStateMachine enemy)
    {
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

    public override void OnDetectionBoxEnter(EnemyStateMachine enemy, Collider2D col)
    {

    }

    public override void OnDetectionBoxStay(EnemyStateMachine enemy, Collider2D col)
    {

    }
    public override void OnDetectionBoxExit(EnemyStateMachine enemy, Collider2D col)
    {

    }

    public override void OnTakeDamage(EnemyStateMachine enemy, float damage, Vector2 push)
    {

    }
}
