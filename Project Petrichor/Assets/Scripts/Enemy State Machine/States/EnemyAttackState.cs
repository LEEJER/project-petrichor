using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    NextState nextState = NextState.Nothing;
    private bool startAttack = false;
    private float time = 0f;
    private float maxTime = 0.6f;
    private bool cooldown = false;
    Collider2D hurtbox;

    public override void EnterState(EnemyStateMachine enemy)
    {
        enemy.currentState = EnemyStateMachine.CurrentState.Attack;
        nextState = NextState.Nothing;
        startAttack = true;
        cooldown = false;
        hurtbox = enemy.transform.Find("Hurtbox").GetComponent<Collider2D>();
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
            case NextState.Deflected:
                enemy.SwitchState(enemy.DeflectedState);
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
                hurtbox.enabled = true;
                enemy.VelocityVector = (enemy.PathfindingTarget - enemy.EnemyRigidbody.position).normalized * 3f;
                cooldown = true;
                time += Time.fixedDeltaTime;
                // automatically go into the chase state after finishing an attack
            }
            else
            {
                nextState = NextState.Chase;
                hurtbox.enabled = false;
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
        hurtbox.enabled = false;
    }

    public override void OnHitboxEnter(EnemyStateMachine enemy, Collider2D collision, string selfComponent)
    {
        GameObject other = collision.gameObject;
        // if our hitbox was hit
        if (selfComponent == "Hitbox")
        {
            // if we are hit by the player
            if (other.layer == LayerMask.NameToLayer("Player"))
            {
                // it was the player's sword
                if (other.CompareTag("Hurtbox"))
                {
                    Sword sword = other.GetComponent<Sword>();
                    // take damage
                    enemy.Health -= sword.damage;

                    // if we just lunged and are in recovery
                    if (cooldown || enemy.Health <= 0)
                    {
                        // take knockback
                        enemy.VelocityVector += sword.dir.normalized * sword.knockbackForce * enemy.KnockbackResistance;
                        cooldown = false;
                        // goto hit state
                        nextState = NextState.Hit;
                    }
                }
            }
        }
        else if (selfComponent == "Hurtbox")
        {
            if (other.layer == LayerMask.NameToLayer("Player"))
            {
                if (other.CompareTag("DeflectBox"))
                {
                    nextState = NextState.Deflected;
                    hurtbox.enabled = false;
                    enemy.VelocityVector = -enemy.VelocityVector.normalized * other.transform.parent.GetComponent<PlayerStateMachine>().DeflectKnockback * enemy.KnockbackResistance;
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
}
