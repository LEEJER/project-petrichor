using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : PlayerState
{
    NextState nextState = NextState.Nothing;
    public override void EnterState(PlayerStateMachine player)
    {
        player.currentState = PlayerStateMachine.CurrentState.Idle;
        nextState = NextState.Nothing;
    }

    public override void UpdateState(PlayerStateMachine player)
    {
        if (player.Health >= player.MaxHealth)
        {
            nextState = NextState.Die;
        }
        switch (nextState)
        {
            case NextState.Deflect:
                player.SwitchState(player.DeflectState);
                break;
            case NextState.Attack:
                player.SwitchState(player.AttackState);
                break;
            case NextState.Dash:
                player.SwitchState(player.DashState);
                break;
            case NextState.Hit:
                player.SwitchState(player.HitState);
                break;
            case NextState.Die:
                player.SwitchState(player.DieState);
                break;
            default:
                if (player.InputVector != Vector2.zero)
                {
                    player.SwitchState(player.RunState);
                }
                break;
        }

        //if (player.InputVector != Vector2.zero)
        //{
        //    player.SwitchState(player.RunState);
        //}
        Animate(player);
    }

    public override void ExitState(PlayerStateMachine player)
    {
        nextState = NextState.Nothing;
    }

    public override void OnMove(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.FacingVector = context.ReadValue<Vector2>();
        }
        else if (context.canceled) { }
    }
    public override void OnFire(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started && nextState != NextState.Hit)
        {
            nextState = NextState.Attack;
        }
        else if (context.performed) { }
        else if (context.canceled) { }
    }
    public override void OnDash(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started && nextState != NextState.Hit)
        {
            nextState = NextState.Dash;
        }
    }
    public override void OnDeflect(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started && nextState != NextState.Hit)
        {
            nextState = NextState.Deflect;
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
                EnemyStateMachine enemy = other.transform.parent.GetComponent<EnemyStateMachine>();
                player.GetHit(enemy);
                // interrupt attacks
                // goto hit state
                nextState = NextState.Hit;
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
    }
}
