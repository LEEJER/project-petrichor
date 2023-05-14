using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHitState : PlayerState
{
    NextState nextState = NextState.Nothing;
    private bool canInterrupt = false;
    public override void EnterState(PlayerStateMachine player)
    {
        player.currentState = PlayerStateMachine.CurrentState.Hit;
        Animate(player);
        canInterrupt = false;
    }

    public override void UpdateState(PlayerStateMachine player)
    {
        if (canInterrupt)
        {
            if (player.Health >= player.MaxHealth)
            {
                nextState = NextState.Die;
            }
            switch (nextState)
            {
                case NextState.Die:
                    //player.SwitchState()
                    break;
                case NextState.Attack:
                    player.SwitchState(player.AttackState);
                    break;
                case NextState.Dash:
                    player.SwitchState(player.DashState);
                    break;
                case NextState.Deflect:
                    player.SwitchState(player.DeflectState);
                    break;
                default:
                    if (player.InputVector != Vector2.zero)
                    {
                        player.SwitchState(player.IdleState);
                    }
                    break;
            }
        }
    }

    public override void ExitState(PlayerStateMachine player)
    {
        canInterrupt = false;
        nextState = NextState.Nothing;
    }

    public override void OnMove(PlayerStateMachine player, InputAction.CallbackContext context)
    {

    }
    public override void OnFire(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            nextState = NextState.Attack;
        }
    }
    public override void OnDash(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            nextState = NextState.Dash;
        }
    }
    public override void OnDeflect(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            nextState = NextState.Deflect;
        }
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
        player.animator.SetTrigger("t_hit");
    }

    public void EventAllowInterrupt()
    {
        canInterrupt = true;
    }

    public void EventEndHit()
    {
        nextState = NextState.Idle;
    }
}
