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

    public abstract void OnHitboxEnter(EnemyStateMachine enemy, Collider2D collision, string selfComponent);
    public abstract void OnHitboxStay(EnemyStateMachine enemy, Collider2D collision, string selfComponent);
    public abstract void OnHitboxExit(EnemyStateMachine enemy, Collider2D collision, string selfComponent);

    //public abstract void OnTakeDamage(EnemyStateMachine enemy, float damage, Vector2 push);
}
