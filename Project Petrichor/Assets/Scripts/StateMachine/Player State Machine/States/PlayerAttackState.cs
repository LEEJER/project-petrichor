using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackState : PlayerState
{
    private bool        canBufferInput  = false;
    private bool        canInterrupt    = false;
    private bool        startAttack     = false;
    private int         attackNum       = 0;
    private Vector2     dir             = Vector2.zero;
    private NextState   nextState       = NextState.Nothing;
    

    public override void EnterState(PlayerStateMachine player)
    {
        player.currentState = PlayerStateMachine.CurrentState.Attack;
        // entering the attack state for the first time
        attackNum       = 0;
        startAttack     = true;
        canInterrupt    = true;
        nextState       = NextState.Nothing;
        canBufferInput  = true;
    }

    public override void UpdateState(PlayerStateMachine player)
    {
        // run the attack
        if (startAttack && canInterrupt)
        {
            dir = player.RelativeMousePos.normalized;
            Animate(player);
            player.sword.SwordAttack(dir, attackNum);
            player.PlayAudioClip(PlayerStateMachine.AudioClips.Swipe);

            nextState       = NextState.Nothing;
            canInterrupt    = false;
            startAttack     = false;
            canBufferInput  = false;

            player.Health += 4f * player.difficultyMultiplier;

            player.VelocityVector   = (player.VelocityVector * 0.5f) + dir.normalized * player.sword.swingMovementSpeed;
            attackNum               = (attackNum + 1) % 2;
        }
        else if (canInterrupt)
        {
            if (player.Health >= player.MaxHealth)
            {
                nextState = NextState.Die;
            }
            switch (nextState)
            {
                case NextState.Deflect:
                    Interrupt(player, player.DeflectState);
                    break;
                case NextState.Dash:
                    Interrupt(player, player.DashState);
                    break;
                case NextState.Hit:
                    Interrupt(player, player.HitState);
                    break;
                case NextState.Die:
                    Interrupt(player, player.DieState);
                    break;
                default:
                    if (player.InputVector != Vector2.zero)
                    {
                        player.FacingVector = player.InputVector;
                        Interrupt(player, player.IdleState);
                    }
                    break;
            }
        }

    }

    public override void ExitState(PlayerStateMachine player)
    {
        startAttack     = false;
        canInterrupt    = true;
        nextState       = NextState.Nothing;
        canBufferInput  = false;
    }

    public override void OnMove(PlayerStateMachine player, InputAction.CallbackContext context)
    {

    }
    public override void OnFire(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canBufferInput && nextState != NextState.Hit) { startAttack = true; }
        }
    }
    public override void OnDash(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canBufferInput && nextState != NextState.Hit) { nextState = NextState.Dash; }
        }
    }
    public override void OnDeflect(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canBufferInput && nextState != NextState.Hit) { nextState = NextState.Deflect; }
        }
    }
    public override void OnHitboxEnter(PlayerStateMachine player, Collider2D collision, string selfComponent)
    {
        GameObject other = collision.gameObject;
        if (selfComponent == "Sword")
        {
            // if we hit an enemy, specifically the hitbox
            if (other.layer == LayerMask.NameToLayer("Enemy") && other.CompareTag("Hitbox"))
            {

                player.PlayAudioClip(PlayerStateMachine.AudioClips.Slice);
                EnemyStateMachine.CurrentState state = other.transform.parent.GetComponent<EnemyStateMachine>().currentState;
                if (state != EnemyStateMachine.CurrentState.Hit)
                {
                    player.Health -= 3f;
                }
                // only apply large knockback if enemy was not deflected
                if (state != EnemyStateMachine.CurrentState.Deflected)
                {
                    // apply self knockback
                    player.VelocityVector += -player.sword.dir.normalized * player.sword.knockbackForce * player.SelfKnockback;
                }
                else
                {
                    player.VelocityVector += -player.sword.dir.normalized * player.sword.knockbackForce * player.SelfKnockback * 0.2f;
                }
            }
        }
        else if (selfComponent == "Hitbox")
        {
            // if our hitbox was hit by enemy hurtbox
            if (other.layer == LayerMask.NameToLayer("Enemy") && other.CompareTag("Hurtbox"))
            {
                EnemyStateMachine enemy = other.transform.parent.GetComponent<EnemyStateMachine>();
                player.GetHit(enemy);
                // interrupt attacks
                canInterrupt = true;
                player.sword.StopAttack();
                // goto hit state
                nextState = NextState.Hit;
            }
        }
    }
    public override void OnHitboxStay(PlayerStateMachine player, Collider2D collision, string selfComponent)
    {

    }
    public override void OnHitboxExit(PlayerStateMachine player, Collider2D collision, string selfComponent)
    {

    }

    private void Animate(PlayerStateMachine player)
    {
        player.FacingVector = dir;
        player.animator.SetTrigger("t_swordAttack");
        player.animator.SetInteger("swordAttack_num", attackNum);
        player.animator.SetFloat("swordAttack_dir_x", dir.x);
        player.animator.SetFloat("swordAttack_dir_y", dir.y);
    }

    private void Interrupt(PlayerStateMachine player, PlayerState state)
    {
        player.animator.SetTrigger("t_interrupt");
        player.SwitchState(state);
    }

    public void EventAllowInterrupt()
    {
        canInterrupt = true;
    }

    public void EventEndSwordAttack(PlayerStateMachine player)
    {
        player.SwitchState(player.IdleState);
    }

    public void EventAllowBuffer(PlayerStateMachine player)
    {
        canBufferInput = true;
    }

    
}
