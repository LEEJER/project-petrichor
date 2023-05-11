using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : PlayerState
{
    public override void EnterState(PlayerStateMachine player)
    {
        
    }

    public override void UpdateState(PlayerStateMachine player)
    {

        Animate(player);
    }

    public override void ExitState(PlayerStateMachine player)
    {

    }

    public override void OnMove(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            player.FacingVector = context.ReadValue<Vector2>();
            player.SwitchState(player.RunState);
        }
        else if (context.canceled) { }
    }
    public override void OnFire(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.SwitchState(player.AttackState);
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
        player.animator.SetFloat("facing_x", player.FacingVector.x);
        player.animator.SetFloat("facing_y", player.FacingVector.y);
    }
}
