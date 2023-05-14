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
    private NextState   startNext       = NextState.Nothing;
    

    public override void EnterState(PlayerStateMachine player)
    {
        player.currentState = PlayerStateMachine.CurrentState.Attack;
        // entering the attack state for the first time
        attackNum       = 0;
        startAttack     = true;
        canInterrupt    = true;
        startNext       = NextState.Nothing;
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

            startNext       = NextState.Nothing;
            canInterrupt    = false;
            startAttack     = false;
            canBufferInput  = false;

            player.VelocityVector   = (player.VelocityVector * 0.5f) + dir.normalized * player.sword.swingMovementSpeed;
            attackNum               = (attackNum + 1) % 2;
        }
        else if (canInterrupt)
        {
            switch (startNext)
            {
                case NextState.Deflect:
                    player.animator.SetTrigger("t_deflect");
                    Interrupt(player, player.DeflectState);
                    break;
                case NextState.Dash:
                    player.animator.SetTrigger("t_dash");
                    Interrupt(player, player.DashState);
                    break;
                default:
                    if (!startAttack && player.InputVector != Vector2.zero)
                    {
                        player.FacingVector = player.InputVector;
                        Interrupt(player, player.IdleState);
                    }
                    break;
            }
        }

        //else if (startNext == NextState.Deflect && canInterrupt)
        //{
        //    player.animator.SetTrigger("t_deflect");
        //    Interrupt(player, player.DeflectState);
        //}
        //else if (startNext == NextState.Dash && canInterrupt)
        //{
        //    player.animator.SetTrigger("t_dash");
        //    Interrupt(player, player.DashState);
        //}
        //else if (player.InputVector != Vector2.zero && canInterrupt)
        //{
        //    player.FacingVector = player.InputVector;
        //    Interrupt(player, player.IdleState);
        //}
    }

    public override void ExitState(PlayerStateMachine player)
    {
        startAttack     = false;
        canInterrupt    = true;
        startNext       = NextState.Nothing;
        canBufferInput  = false;
    }

    public override void OnMove(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        //if (context.started || context.performed)
        //{
        //    if (canInterrupt)
        //    {
        //        Interrupt(player, player.IdleState);
        //    }
        //}
    }
    public override void OnFire(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canBufferInput) { startAttack = true; }
        }
    }
    public override void OnDash(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canBufferInput) { startNext = NextState.Dash; }
        }
    }
    public override void OnDeflect(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canBufferInput) { startNext = NextState.Deflect; }
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
                    // apply self knockback
                    player.VelocityVector += -1f * player.sword.dir.normalized * (player.sword.knockbackForce * player.SelfKnockback);
                // if the enemy is not currently hit
                //if (other.transform.parent.GetComponent<EnemyStateMachine>().currentState != EnemyStateMachine.CurrentState.Hit)
                //{
                //}
            }
        }
        else if (selfComponent == "Hitbox")
        {
            // if our hitbox was hit by enemy hurtbox
            if (other.layer == LayerMask.NameToLayer("Enemy") && other.CompareTag("Hurtbox"))
            {
                // take damage
                // apply self knockback
                // interrupt attacks
                // goto hit state
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
