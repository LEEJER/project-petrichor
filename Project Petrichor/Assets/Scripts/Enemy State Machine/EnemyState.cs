using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState
{
    public enum NextState
    {
        Nothing,
        Idle,
        Patrol,
        Chase,
        Attack,
        Hit,
        Die
    }

    public abstract void EnterState(EnemyStateMachine enemy);

    public abstract void UpdateState(EnemyStateMachine enemy);

    public abstract void ExitState(EnemyStateMachine enemy);

    public abstract void OnDetectionBoxEnter(EnemyStateMachine enemy, Collider2D col);
    public abstract void OnDetectionBoxStay(EnemyStateMachine enemy, Collider2D col);
    public abstract void OnDetectionBoxExit(EnemyStateMachine enemy, Collider2D col);

    public abstract void OnTakeDamage(EnemyStateMachine enemy, float damage, Vector2 push);
}
