using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHitState : PlayerState
{
    public override void EnterState(PlayerStateMachine player)
    {
        Animate(player);
    }

    public override void UpdateState(PlayerStateMachine player)
    {

    }

    public override void ExitState(PlayerStateMachine player)
    {

    }

    public override void OnMove(PlayerStateMachine player, InputAction.CallbackContext context)
    {

    }
    public override void OnFire(PlayerStateMachine player, InputAction.CallbackContext context)
    {

    }
    public override void OnDash(PlayerStateMachine player, InputAction.CallbackContext context)
    {

    }
    public override void OnDeflect(PlayerStateMachine player, InputAction.CallbackContext context)
    {

    }
    public override void OnCollisionEnter2D(PlayerStateMachine player, Collision2D col)
    {

    }

    private void Animate(PlayerStateMachine player)
    {
        player.animator.SetTrigger("t_hit");
    }

    public void EventAllowInterrupt(PlayerStateMachine player)
    {
        player.SwitchState(player.IdleState);
    }
}
