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
    private NextState   nextState       = NextState.Nothing;

    public override void EnterState(PlayerStateMachine player)
    {
        player.currentState = PlayerStateMachine.CurrentState.DeflectHit;
        canInterrupt    = true;
        canBufferInput  = true;
        startDeflectHit = true; 
        nextState       = NextState.Nothing;
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

            nextState = NextState.Nothing;
        }
        else if (canInterrupt)
        {
            switch (nextState)
            {
                case NextState.Attack:
                    Interrupt(player, player.AttackState);
                    break;
                case NextState.Deflect:
                    Interrupt(player, player.DeflectState);
                    break;
                case NextState.Dash:
                    Interrupt(player, player.DashState);
                    break;
                case NextState.Hit:
                    Interrupt(player, player.HitState);
                    break;
                default:
                    if (player.InputVector != Vector2.zero)
                    {
                        player.FacingVector = player.InputVector;
                        Interrupt(player, player.IdleState);
                    }
                    break;
            }
        }

        //else if (nextState == NextState.Attack && canInterrupt)
        //{
        //    player.animator.SetTrigger("t_attack");
        //    Interrupt(player, player.AttackState);
        //}
        //else if (nextState == NextState.Deflect && canInterrupt)
        //{
        //    player.animator.SetTrigger("t_deflect");
        //    Interrupt(player, player.DeflectState);
        //}
        //else if (nextState == NextState.Dash && canInterrupt)
        //{
        //    player.animator.SetTrigger("t_dash");
        //    Interrupt(player, player.DashState);
        //}
        //else if (player.InputVector != Vector2.zero && canInterrupt)
        //{
        //    player.FacingVector = player.InputVector;
        //    Interrupt(player, player.IdleState);
        //}
    }

    public override void ExitState(PlayerStateMachine player)
    {
        canInterrupt = true;
        canBufferInput = false;
        nextState = NextState.Nothing;
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
        if (context.started && nextState != NextState.Hit)
        {
            if (canBufferInput) { nextState = NextState.Attack; }
        }
    }
    public override void OnDash(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started && nextState != NextState.Hit)
        {
            if (canBufferInput) { nextState = NextState.Dash; }
        }
    }
    public override void OnDeflect(PlayerStateMachine player, InputAction.CallbackContext context)
    {
        if (context.started && nextState != NextState.Hit)
        {
            if (canBufferInput) { nextState = NextState.Deflect; }
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
                canInterrupt = true;
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
