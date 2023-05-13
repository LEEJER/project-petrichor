using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    NextState nextState = NextState.Nothing;
    private bool startAttack = false;
    private float time = 0f;
    private float maxTime = 1f;
    private bool cooldown = false;

    public override void EnterState(EnemyStateMachine enemy)
    {
        enemy.currentState = EnemyStateMachine.CurrentState.Attack;
        nextState = NextState.Nothing;
        startAttack = true;
        cooldown = false;
    }

    public override void UpdateState(EnemyStateMachine enemy)
    {
        switch (nextState)
        {
            case NextState.Chase:
                if (!cooldown) { enemy.SwitchState(enemy.ChaseState); }
                break;
            case NextState.Hit:
                enemy.SwitchState(enemy.HitState);
                break;
            default:
                break;
        }

        if (startAttack)
        {
            startAttack = false;
            time += Time.fixedDeltaTime;
        }

        if (time > 0)
        {
            time += Time.fixedDeltaTime;
        }

        if (time > maxTime)
        {
            time = 0;
            if (!cooldown)
            {
                enemy.VelocityVector = (enemy.PathfindingTarget - enemy.EnemyRigidbody.position).normalized * 3f;
                cooldown = true;
                time += Time.fixedDeltaTime;
                // automatically go into the chase state after finishing an attack
                nextState = NextState.Chase;
            }
            else
            {
                cooldown = false;
            }
        }
    }

    public override void ExitState(EnemyStateMachine enemy)
    {
        nextState = NextState.Nothing;
        startAttack = false;
        cooldown = false;
        time = 0;
    }

    public override void OnHitboxEnter(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {
        GameObject other = collision.gameObject;
        // if our hitbox was hit
        if (selfComponent == "Hitbox")
        {
            // if we are hit by the player hurtbox
            if (other.layer == LayerMask.NameToLayer("Player") && other.CompareTag("Hurtbox"))
            {
                Sword sword = other.GetComponent<Sword>();
                // take damage
                enemy.Health -= sword.damage;
                
                // if we just lunged and are in recovery
                if (cooldown)
                {
                    // take knockback
                    enemy.VelocityVector += sword.dir.normalized * sword.knockbackForce;
                    cooldown = false;
                    // goto hit state
                    nextState = NextState.Hit;
                }
            }
        }

        
    }

    public override void OnHitboxStay(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {

    }
    public override void OnHitboxExit(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {

    }

    //public override void OnTakeDamage(EnemyStateMachine enemy, float damage, Vector2 push)
    //{
    //    enemy.Health -= damage;
    //    enemy.VelocityVector += push;
    //    nextState = NextState.Hit;
    //}
}
