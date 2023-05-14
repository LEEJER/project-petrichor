using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDieState : PlayerState
{
    public override void EnterState(PlayerStateMachine player)
    {
        Debug.Log("Player should be dead: " + player.currentState);
        player.currentState = PlayerStateMachine.CurrentState.Die;
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

    public override void OnHitboxEnter(PlayerStateMachine player, Collider2D collision, string selfComponent)
    {

    }
    public override void OnHitboxStay(PlayerStateMachine player, Collider2D collision, string selfComponent)
    {

    }
    public override void OnHitboxExit(PlayerStateMachine player, Collider2D collision, string selfComponent)
    {

    }

    private void Animate(PlayerStateMachine player)
    {
        player.animator.SetTrigger("t_die");
    }

    public void EventAllowInterrupt()
    {

    }

    public void EventEndDie(PlayerStateMachine player)
    {
        player.DestroyThisObject();
    }
}
