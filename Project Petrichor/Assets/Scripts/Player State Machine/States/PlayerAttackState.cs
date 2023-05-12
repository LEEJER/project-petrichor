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

            canInterrupt    = false;
            startAttack     = false;
            canBufferInput  = false;
            startNext = NextState.Nothing;

            player.sword.SwordAttack(dir, attackNum);
            player.VelocityVector = (player.VelocityVector * 0.5f) + dir.normalized * player.sword.swingMovementSpeed;
            attackNum = (attackNum + 1) % 2;
        }

        else if (startNext == NextState.Deflect && canInterrupt)
        {
            player.animator.SetTrigger("t_deflect");
            Interrupt(player, player.DeflectState);
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
        startAttack     = false;
        canInterrupt    = true;
        startNext       = NextState.Nothing;
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
    public override void OnCollisionEnter2D(PlayerStateMachine player, Collision2D col)
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

    public void EventAllowBuffer()
    {
        canBufferInput = true;
    }
}
