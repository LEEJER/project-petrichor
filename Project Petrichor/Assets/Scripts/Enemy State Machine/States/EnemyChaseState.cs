using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    NextState   nextState = NextState.Nothing;
    float timeSinceLastPath = 0f;
    float pathUpdateInterval = 0.1f;
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

        timeSinceLastPath += Time.fixedDeltaTime;
        if (timeSinceLastPath > pathUpdateInterval)
        {
            timeSinceLastPath = 0f;
            enemy.FindPathTo(enemy.PathfindingTarget);
        }

        // if there is a path and if we are not at the end of this path
        if (enemy.Path != null && enemy.CurrentWaypoint < enemy.Path.vectorPath.Count)
        {
            Vector2 pathfindingDirection = ((Vector2)enemy.Path.vectorPath[enemy.CurrentWaypoint] - enemy.EnemyRigidbody.position).normalized;
            enemy.VelocityVector = pathfindingDirection * enemy.ChaseSpeed;

            float distance = Vector2.Distance(enemy.EnemyRigidbody.position, enemy.Path.vectorPath[enemy.CurrentWaypoint]);
            Debug.Log(distance);
            if (distance < enemy.NextWaypointDistance)
            {
                enemy.CurrentWaypoint++;
            }
        }
    }

    public override void ExitState(EnemyStateMachine enemy)
    {
        Debug.Log("Exiting chase state");
        nextState = NextState.Nothing;
    }

    public override void OnDetectionBoxEnter(EnemyStateMachine enemy, Collider2D collision)
    {

    }

    public override void OnDetectionBoxStay(EnemyStateMachine enemy, Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enemy.PathfindingTarget = collision.gameObject.GetComponent<Rigidbody2D>().position;
        }
    }
    public override void OnDetectionBoxExit(EnemyStateMachine enemy, Collider2D collision)
    {
        nextState = NextState.Idle;
        enemy.Path = null;
    }

    public override void OnTakeDamage(EnemyStateMachine enemy, float damage, Vector2 push)
    {
        enemy.Health -= damage;
        enemy.VelocityVector += push;
        nextState = NextState.Hit;
    }
}
