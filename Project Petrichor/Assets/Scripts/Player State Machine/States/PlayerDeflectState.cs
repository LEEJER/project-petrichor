using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeflectState : PlayerState
{
    private bool        deflectFramesActive = false;
    private bool        canBufferInput  = false;
    private bool        canInterrupt    = false;
    private bool        startDeflect    = false;
    private bool        startDeflectHit = false;
    private Vector2     dir             = Vector2.zero;
    private NextState   startNext       = NextState.Nothing;
    public override void EnterState(PlayerStateMachine player)
    {
        deflectFramesActive = false;
        startDeflect    = true;
        canInterrupt    = true;
        startDeflectHit = false;
        startNext       = NextState.Nothing;
        canBufferInput = true;
    }

    public override void UpdateState(PlayerStateMachine player)
    {
        if (startDeflectHit)
        {
            canInterrupt = false;
            startDeflect = false;
            canBufferInput = false;
            startDeflectHit = false;

            player.SwitchState(player.DeflectHitState);

        }
        else if (startDeflect && canInterrupt)
        {
            deflectFramesActive = true;
            canInterrupt = false;
            startDeflect = false;
            canBufferInput = false;
            //startDeflectHit = false;

            dir = player.RelativeMousePos.normalized;
            Animate(player);

            startNext = NextState.Nothing;
            player.VelocityVector = (player.VelocityVector * 0.75f);
        }
        else if (startNext == NextState.Attack && canInterrupt)
        {
            player.animator.SetTrigger("t_swordAttack");
            Interrupt(player, player.AttackState);
        }
        else if (startNext == NextState.Dash && canInterrupt)
        {
            player.animator.SetTrigger("t_dash");
            Interrupt(player, player.DashState);
        }
        else if (player.InputVector != Vector2.zero && canInterrupt)
        {
            player.FacingVector = player.InputVector;
            Interrupt(player, player.IdleState);
        }
    }

    public override void ExitState(PlayerStateMachine player)
    {
        startDeflect = false;
        canInterrupt = true;
        startDeflectHit = false;
        startNext = NextState.Nothing;
        deflectFramesActive = false;
        canBufferInput = false;
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
            if (canBufferInput) { startNext = NextState.Attack; }
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
            if (canBufferInput) { startDeflect = true; }
        }
    }
    public override void OnCollisionEnter2D(PlayerStateMachine player, Collision2D col)
    {
        // we can interrupt once the deflect ends
        // so, if we are still in the deflect animation
        if (deflectFramesActive)
        {
            if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Enemy"))
            {
                startDeflectHit = true;
                // apply player velocity
            }
        }
        // we are not in the deflect animation
        else
        {

        }
    }

    private void Animate(PlayerStateMachine player)
    {
        player.FacingVector = dir;
        player.animator.SetTrigger("t_deflect");
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

    public void EventEndDeflect(PlayerStateMachine player)
    {
        player.SwitchState(player.IdleState);
    }

    public void EventAllowBuffer()
    {
        canBufferInput = true;
    }

    public void EventRemoveDeflectFrames()
    {
        deflectFramesActive = false;
    }
}
