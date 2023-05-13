using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashState : PlayerState
{
    private bool canInterrupt = false;
    private bool canStartDash = false;
    private bool startDash = false;
    private NextState startNext = NextState.Nothing;
    public override void EnterState(PlayerStateMachine player)
    {
        player.currentState = PlayerStateMachine.CurrentState.Dash;
        startDash = true;
        canInterrupt = true;
        canStartDash = true;
        startNext = NextState.Nothing;
    }

    public override void UpdateState(PlayerStateMachine player)
    {
        if (startDash && canInterrupt && canStartDash)
        {
            canInterrupt    = false;
            startDash       = false;
            startNext = NextState.Nothing;
            canStartDash    = false;

            if (player.InputVector != Vector2.zero) { player.FacingVector = player.InputVector; }
            player.VelocityVector = player.FacingVector.normalized * player.DashSpeed;

            Animate(player);
        }
        else if (startNext == NextState.Attack && canInterrupt)
        {
            player.animator.SetTrigger("t_interrupt");
            player.SwitchState(player.AttackState);
        }
        else if (startNext == NextState.Deflect && canInterrupt)
        {
            player.animator.SetTrigger("t_interrupt");
            player.SwitchState(player.DeflectState);
        }
        // if there is movement input, try to interrupt
        else if (canStartDash && player.InputVector != Vector2.zero)
        {
            player.FacingVector = player.InputVector;
            player.animator.SetTrigger("t_interrupt");
            player.SwitchState(player.IdleState);
        }
    }

    public override void ExitState(PlayerStateMachine player)
    {
        canInterrupt    = false;
        startDash       = false;
        canStartDash    = false;
    }

    public override void OnMove(PlayerStateMachine player, InputAction.CallbackContext context)
    {
       
    }
    public override void OnFire(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            startNext = NextState.Attack;
        }
    }
    public override void OnDash(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            startDash = true;
        }
    }
    public override void OnDeflect(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            startNext = NextState.Deflect;
        }
    }


    public override void OnHitboxEnter(PlayerStateMachine player, Collider2D collision, string selfComponent)
    {
        GameObject other = collision.gameObject;
        if (selfComponent == "Hitbox")
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
        player.animator.SetFloat("facing_x", player.FacingVector.x);
        player.animator.SetFloat("facing_y", player.FacingVector.y);
        player.animator.SetTrigger("t_dash");
    }

    public void EventAllowInterrupt()
    {
        canInterrupt = true;
    }

    public void EventAllowNewDash()
    {
        canStartDash = true;
    }

    public void EventEndDash(PlayerStateMachine player)
    {
        player.SwitchState(player.IdleState);
    }
}
