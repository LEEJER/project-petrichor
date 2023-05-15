using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public NextState nextState = NextState.Nothing;
    private float patrolTime = 0f;
    private float waitToIdle = 0f;
    private float softPatrolDistanceFromSpawn = 0.66f;
    private float hardPatrolDistanceFromSpawn = 1f;
    private Vector2 patrolDirection = Vector2.right;
    private float preferredTuringDirection = -1;
    public override void EnterState(EnemyStateMachine enemy)
    {
        enemy.currentState = EnemyStateMachine.CurrentState.Patrol;
        nextState = NextState.Nothing;
        patrolTime = 0f;
        waitToIdle = Random.Range(0.66f, 2f);

        // reset the patrol direction
        patrolDirection = Vector2.right;
        // rotate the vector2 to a random angle
        if (Vector2.Distance(enemy.SpawnPoint, enemy.EnemyRigidbody.position) >= hardPatrolDistanceFromSpawn)
        {
            patrolDirection = (enemy.SpawnPoint - enemy.EnemyRigidbody.position).normalized;
        }
        else
        {
            RotatePatrolDirectionByAngle(Random.Range(0f, 360f));
        }
        preferredTuringDirection = Mathf.Sign(Vector2.Angle((enemy.EnemyRigidbody.position - enemy.SpawnPoint).normalized, patrolDirection));
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        switch (nextState)
        {
            case NextState.Chase:
                enemy.SwitchState(enemy.ChaseState);
                return;
            case NextState.Hit:
                enemy.SwitchState(enemy.HitState);
                return;
            case NextState.Idle:
                enemy.SwitchState(enemy.IdleState);
                return;
            default:
                break;
        }
        patrolTime += Time.fixedDeltaTime;
        if (patrolTime >= waitToIdle)
        {
            nextState = NextState.Idle;
            return;
        }

        if (Vector2.Distance(enemy.SpawnPoint, enemy.EnemyRigidbody.position) > softPatrolDistanceFromSpawn)
        {
            float angleToPointTowardsSpawn = Vector2.SignedAngle(patrolDirection, enemy.SpawnPoint - enemy.EnemyRigidbody.position);
            RotatePatrolDirectionByAngle(preferredTuringDirection * angleToPointTowardsSpawn * Time.deltaTime);
        }
        enemy.VelocityVector = enemy.GetChaseVector(enemy.EnemyRigidbody.position + patrolDirection, enemy.PatrolMovementSpeed);
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

    private void RotatePatrolDirectionByAngle(float angle)
    {
        patrolDirection = Quaternion.AngleAxis(angle, Vector3.forward) * patrolDirection;
    }
}
