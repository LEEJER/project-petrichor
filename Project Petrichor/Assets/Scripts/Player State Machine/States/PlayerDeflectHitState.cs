using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeflectHitState : PlayerState
{
    private bool        canBufferInput  = false;
    private bool        canInterrupt    = false;
    private bool        startDeflectHit = false;
    private Vector2     dir             = Vector2.zero;
    private NextState   startNext       = NextState.Nothing;

    public override void EnterState(PlayerStateMachine player)
    {
        player.currentState = PlayerStateMachine.CurrentState.DeflectHit;
        canInterrupt    = true;
        canBufferInput  = true;
        startDeflectHit = true; 
        startNext       = NextState.Nothing;
        dir             = player.VelocityVector * -1f;
        Animate(player);
    }

    public override void UpdateState(PlayerStateMachine player)
    {
        if (startDeflectHit)
        {
            canInterrupt = false;
            canBufferInput = false;

            Animate(player);

            startNext = NextState.Nothing;
        }
        else if (startNext == NextState.Attack && canInterrupt)
        {
            player.animator.SetTrigger("t_attack");
            Interrupt(player, player.AttackState);
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
        canInterrupt = true;
        canBufferInput = false;
        startNext = NextState.Nothing;
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
            if (canBufferInput) { startNext = NextState.Attack; }
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
    //public override void OnCollisionEnter2D(PlayerStateMachine player, Collision2D col)
    //{
    //    // we can interrupt once the deflect ends
    //    // so, if we are still in the deflect animation
    //    if (!canInterrupt)
    //    {
    //        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("EnemyProjectile"))
    //        {
    //            startDeflectHit = true;
    //            // apply player velocity
    //        }
    //    }
    //    // we are not in the deflect animation
    //    else
    //    {

    //    }
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
                Debug.Log("Player was hit by enemy in DeflectHitState");
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
        player.FacingVector = dir;
        player.animator.SetTrigger("t_deflect_hit");
        player.animator.SetFloat("facing_x", dir.x);
        player.animator.SetFloat("facing_y", dir.y);
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

    public void EventEndDeflectHit(PlayerStateMachine player)
    {
        player.SwitchState(player.IdleState);
    }

    public void EventAllowBuffer()
    {
        canBufferInput = true;
    }
}
