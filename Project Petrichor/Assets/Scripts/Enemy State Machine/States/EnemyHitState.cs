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
        enemy.currentState = EnemyStateMachine.CurrentState.Hit;
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

    public override void OnHitboxEnter(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {
        GameObject other = collision.gameObject;
        // if we were hit
        if (selfComponent == "Hitbox")
        {
            // by the player hurtbox
            if (other.layer == LayerMask.NameToLayer("Player") && other.CompareTag("Hurtbox"))
            {
                Sword sword = other.GetComponent<Sword>();
                // take damage
                enemy.Health -= sword.damage;
                // take knockback
                enemy.VelocityVector += sword.dir.normalized * sword.knockbackForce;
                // go to hit state
                nextState = NextState.Hit;
            }
        }
        // if we detected a player
        else if (selfComponent == "DetectionBox")
        {
            // make sure it was a player
            if (other.layer == LayerMask.NameToLayer("Player") && collision.gameObject.CompareTag("Player"))
            {
                nextState = NextState.Chase;
            }
        }
    }

    public override void OnHitboxStay(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {
        if (selfComponent == "DetectionBox")
        {
            // make sure it was a player
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && collision.gameObject.CompareTag("Player"))
            {
                nextState = NextState.Chase;
            }
        }
    }

    public override void OnHitboxExit(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {

    }

    //public override void OnTakeDamage(EnemyStateMachine enemy, float damage, Vector2 push)
    //{

    //}
}
