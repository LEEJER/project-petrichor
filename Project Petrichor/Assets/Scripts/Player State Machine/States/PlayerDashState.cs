using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashState : PlayerState
{
    private bool canInterrupt = false;
    private bool startDash = false;
    private bool startAttack = false;
    public override void EnterState(PlayerStateMachine player)
    {
        startDash = true;
        canInterrupt = true;
        startAttack = false;
    }

    public override void UpdateState(PlayerStateMachine player)
    {
        if (startDash && canInterrupt)
        {
            canInterrupt    = false;
            startDash       = false;
            startAttack     = false;

            if (player.InputVector != Vector2.zero) { player.FacingVector = player.InputVector; }
            player.VelocityVector = player.FacingVector.normalized * player.DashSpeed;

            Animate(player);
        }
        else if (startAttack && canInterrupt)
        {
            player.animator.SetTrigger("t_interrupt");
            player.SwitchState(player.AttackState);
        }
        // if there is movement input, try to interrupt
        else if (canInterrupt && player.InputVector != Vector2.zero)
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
    }

    public override void OnMove(PlayerStateMachine player, InputAction.CallbackContext context)
    {
       
    }
    public override void OnFire(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            startAttack = true;
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

    public void EventEndDash(PlayerStateMachine player)
    {
        //startDash = false;
        //canInterrupt = false;
        player.SwitchState(player.IdleState);
    }
}
