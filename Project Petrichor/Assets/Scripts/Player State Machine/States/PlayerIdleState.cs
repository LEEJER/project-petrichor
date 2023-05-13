using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : PlayerState
{
    public override void EnterState(PlayerStateMachine player)
    {
        player.currentState = PlayerStateMachine.CurrentState.Idle;
    }

    public override void UpdateState(PlayerStateMachine player)
    {
        if (player.InputVector != Vector2.zero)
        {
            player.SwitchState(player.RunState);
        }
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
    //public override void OnCollisionEnter2D(PlayerStateMachine player, Collision2D col)
    //{

    //}
    public override void OnHitboxEnter(PlayerStateMachine player, Collider2D collision, string selfComponent)
    {
        GameObject other = collision.gameObject;
        if (selfComponent == "Hitbox")
        {
            // if our hitbox was hit by enemy hurtbox
            if (other.layer == LayerMask.NameToLayer("Enemy") && other.CompareTag("Hurtbox"))
            {
                // take damage
                // apply self knockback
                // interrupt attacks
                // goto hit state
                Debug.Log("Player was hit by enemy in IdleState");
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
