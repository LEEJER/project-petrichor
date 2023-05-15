using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    NextState   nextState           = NextState.Nothing;
    float       timeSinceLastPath   = 0f;
    float       pathUpdateInterval  = 0.12f;
    float       distanceToPlayer    = 0f;
    private bool foundTarget = false;
    public override void EnterState(EnemyStateMachine enemy)
    {
        foundTarget = false;
        enemy.currentState = EnemyStateMachine.CurrentState.Chase;
        nextState = NextState.Nothing;
        distanceToPlayer = 0f;
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        switch (nextState)
        {
            case NextState.Idle:
                enemy.SwitchState(enemy.IdleState);
                return;
            case NextState.Attack:
                enemy.SwitchState(enemy.AttackState);
                return;
            case NextState.Hit:
                enemy.SwitchState(enemy.HitState);
                return;
            default:
                break;
        }

        if (foundTarget)
        {
            bool canSeePlayer = CanSeePlayer(enemy);
            // if we can attack, we should
            if (canSeePlayer && distanceToPlayer < enemy.AttackDistance)
            {
                nextState = NextState.Attack;
                return;
            }


            timeSinceLastPath += Time.fixedDeltaTime;
            if (timeSinceLastPath > pathUpdateInterval && !canSeePlayer)
            {
                timeSinceLastPath = 0f;
                enemy.FindPathTo(enemy.PathfindingTarget);
            }

            // let the decision vectors take over
            if (canSeePlayer)
            {
                enemy.VelocityVector = enemy.GetChaseVector(enemy.PathfindingTarget, enemy.ChaseSpeed);
            }
            // if there is a path and if we are not at the end of this path
            else if (enemy.Path != null)
            {
                FollowPath(enemy);
            }

            else
            {

            }
        } 
        else
        {
            nextState = NextState.Idle;
        }
    }

    public override void ExitState(EnemyStateMachine enemy)
    {
        nextState = NextState.Nothing;
        distanceToPlayer = 0f;
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
                enemy.VelocityVector += sword.dir.normalized * sword.knockbackForce * enemy.KnockbackResistance/2;
                // go to hit state (tank the firstt hit)
                if (enemy.Health < enemy.Health / 2)
                {
                    nextState = NextState.Hit;
                }
            }
        } 
        else if (selfComponent == "DetectionBox")
        {
            if (other.CompareTag("Player"))
            {
                foundTarget = true;
                enemy.PathfindingTarget = collision.gameObject.GetComponent<Rigidbody2D>().position;
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
                foundTarget = true;
                enemy.PathfindingTarget = collision.gameObject.GetComponent<Rigidbody2D>().position;
            }
        }
    }
    public override void OnHitboxExit(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {
        GameObject other = collision.gameObject;
        if (selfComponent == "DetectionBox")
        {
            if (other.CompareTag("Player"))
            {
                foundTarget = false;
                nextState = NextState.Idle;
                enemy.Path = null;
            }
        }
    }

    private bool CanSeePlayer(EnemyStateMachine enemy)
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Environment"));

        Vector2 direction = (enemy.PathfindingTarget - enemy.EnemyRigidbody.position).normalized;
        distanceToPlayer = Vector2.Distance(enemy.PathfindingTarget, enemy.EnemyRigidbody.position);
        RaycastHit2D[] results = new RaycastHit2D[1];

        int count = Physics2D.Raycast(enemy.EnemyRigidbody.position, direction, contactFilter, results, distanceToPlayer);
        return (count == 0);
    }

    private void FollowPath(EnemyStateMachine enemy)
    {
        if (enemy.CurrentWaypoint < enemy.Path.vectorPath.Count)
        {
            Vector2 pathfindingDirection = ((Vector2)enemy.Path.vectorPath[enemy.CurrentWaypoint] - enemy.EnemyRigidbody.position).normalized;
            enemy.VelocityVector = pathfindingDirection * enemy.ChaseSpeed;

            float distance = Vector2.Distance(enemy.EnemyRigidbody.position, enemy.Path.vectorPath[enemy.CurrentWaypoint]);
            //Debug.Log(distance);
            if (distance < enemy.NextWaypointDistance)
            {
                enemy.CurrentWaypoint++;
            }
        }
        
    }

}