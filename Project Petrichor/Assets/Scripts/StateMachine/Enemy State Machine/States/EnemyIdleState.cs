using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public NextState nextState = NextState.Nothing;
    private float idleTime = 0f;
    private float waitToPatrol = 0f;
    public override void EnterState(EnemyStateMachine enemy)
    {
        enemy.currentState = EnemyStateMachine.CurrentState.Idle;
        nextState = NextState.Nothing;
        idleTime = 0f;
        waitToPatrol = Random.Range(1f, 3f);
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        switch (nextState)
        {
            case NextState.Chase:
                enemy.SwitchState(enemy.ChaseState);
                break;
            case NextState.Hit:
                enemy.SwitchState(enemy.HitState);
                break;
            case NextState.Patrol:
                enemy.SwitchState(enemy.PatrolState);
                break;
            default:
                break;
        }
        idleTime += Time.fixedDeltaTime;
        if (idleTime >= waitToPatrol)
        {
            nextState = NextState.Patrol;
        }
    }

    public override void ExitState(EnemyStateMachine enemy)
    {
        nextState = NextState.Nothing;
    }

    public override void OnHitboxEnter(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {
        GameObject other = collision.gameObject;
        // if our hitbox is hit
        if (selfComponent == "Hitbox")
        {
            // if we are hit by the player hurtbox
            if (other.layer == LayerMask.NameToLayer("Player") && other.CompareTag("Hurtbox"))
            {
                Sword sword = other.GetComponent<Sword>();
                // take damage
                enemy.Health -= sword.damage;
                // take knockback
                enemy.VelocityVector += sword.dir.normalized * sword.knockbackForce * enemy.KnockbackResistance;
                // go to hit state
                nextState = NextState.Hit;
            }
        }
        else if (selfComponent == "DetectionBox")
        {
            if (other.CompareTag("Player"))
            {
                enemy.PathfindingTarget = collision.gameObject.GetComponent<Rigidbody2D>().position;
                nextState = NextState.Chase;
            }
        }
    }

    public override void OnHitboxStay(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {
        GameObject other = collision.gameObject;
        if (selfComponent == "DetectionBox")
        {
            if (other.CompareTag("Player"))
            {
                enemy.PathfindingTarget = other.GetComponent<Rigidbody2D>().position;
                nextState = NextState.Chase;
            }
        }
    }

    public override void OnHitboxExit(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {

    }
}
