using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunState : PlayerState
{
    public override void EnterState(PlayerStateMachine player)
    {
        
    }

    public override void UpdateState(PlayerStateMachine player)
    {
        if (player.InputVector != Vector2.zero)
        {
            player.VelocityVector = player.InputVector * player.MovementSpeed;
            Animate(player);
        }
        else
        {
            player.SwitchState(player.IdleState);
        }
    }

    public override void ExitState(PlayerStateMachine player)
    {
        player.animator.SetFloat("velocity_magnitude", 0f);
    }

    public override void OnMove(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            player.FacingVector = context.ReadValue<Vector2>();
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
        if (context.started)
        {
            player.SwitchState(player.DashState);
        }
    }
    public override void OnDeflect(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.SwitchState(player.DeflectState);
        }
    }
    public override void OnCollisionEnter2D(PlayerStateMachine player, Collision2D col)
    {

    }

    private void Animate(PlayerStateMachine player)
    {
        player.animator.SetFloat("facing_x", player.FacingVector.x);
        player.animator.SetFloat("facing_y", player.FacingVector.y);
        player.animator.SetFloat("velocity_magnitude", player.VelocityVector.magnitude);
    }
}
