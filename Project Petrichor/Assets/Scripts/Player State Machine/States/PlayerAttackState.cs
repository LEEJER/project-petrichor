using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackState : PlayerState
{
    private bool canBufferInput = false;
    private bool canInterrupt = false;
    private bool startAttack = false;
    private bool endAttack = false;
    private int attackNum = 0;
    private Vector2 dir = Vector2.zero;

    public override void EnterState(PlayerStateMachine player)
    {
        // entering the attack state for the first time
        attackNum       = 0;
        startAttack     = true;
        endAttack       = false;
        canInterrupt    = true;
    }

    public override void UpdateState(PlayerStateMachine player)
    {
        if (canInterrupt) { player.animator.SetTrigger("t_swordAttackNext"); }
        // run the attack
        if (startAttack && canInterrupt)
        {
            dir = player.RelativeMousePos.normalized;
            Animate(player);

            canInterrupt    = false;
            startAttack     = false;
            canBufferInput  = false;
            endAttack       = false;

            player.sword.SwordAttack(dir, attackNum);
            player.VelocityVector = (player.VelocityVector * 0.5f) + dir.normalized * player.sword.swingMovementSpeed;
            attackNum = (attackNum + 1) % 2;
        }
        else if (endAttack)
        {
            // end attack, return to idle
            player.SwitchState(player.IdleState);
        }
    }

    public override void ExitState(PlayerStateMachine player)
    {

    }

    public override void OnMove(PlayerStateMachine player, InputAction.CallbackContext context)
    {

    }
    public override void OnFire(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canBufferInput) { startAttack = true; }
        }
        else if (context.performed) { }
        else if (context.canceled) { }
    }
    public override void OnDash(PlayerStateMachine player, InputAction.CallbackContext context)
    {

    }
    public override void OnDeflect(PlayerStateMachine player, InputAction.CallbackContext context)
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

    public void EventAllowInterrupt()
    {
        canInterrupt = true;
    }

    public void EventEndSwordAttack()
    {
        startAttack = false;
        canInterrupt = true;
        endAttack = true;
    }

    public void EventAllowBuffer()
    {
        canBufferInput = true;
    }
}
