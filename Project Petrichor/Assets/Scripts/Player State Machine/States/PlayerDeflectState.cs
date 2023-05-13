using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeflectState : PlayerState
{
    private bool        deflectFramesActive = false;
    private bool        canBufferInput  = false;
    private bool        canInterrupt    = false;
    private bool        startDeflect    = false;
    private bool        startDeflectHit = false;
    private Vector2     dir             = Vector2.zero;
    private NextState   startNext       = NextState.Nothing;
    public override void EnterState(PlayerStateMachine player)
    {
        player.currentState = PlayerStateMachine.CurrentState.Deflect;
        deflectFramesActive = false;
        startDeflect    = true;
        canInterrupt    = true;
        startDeflectHit = false;
        startNext       = NextState.Nothing;
        canBufferInput = true;
        dir = player.RelativeMousePos.normalized;
    }

    public override void UpdateState(PlayerStateMachine player)
    {
        if (startDeflectHit)
        {
            canInterrupt = false;
            startDeflect = false;
            canBufferInput = false;
            startDeflectHit = false;

            player.SwitchState(player.DeflectHitState);

        }
        else if (startDeflect && canInterrupt)
        {
            deflectFramesActive = true;
            canInterrupt = false;
            startDeflect = false;
            canBufferInput = false;
            //startDeflectHit = false;

            
            Animate(player);

            startNext = NextState.Nothing;
            player.VelocityVector = (player.VelocityVector * 0.75f);
        }
        else if (startNext == NextState.Attack && canInterrupt)
        {
            player.animator.SetTrigger("t_swordAttack");
            Interrupt(player, player.AttackState);
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
        startDeflect = false;
        canInterrupt = true;
        startDeflectHit = false;
        startNext = NextState.Nothing;
        deflectFramesActive = false;
        canBufferInput = false;
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
            if (canBufferInput) { 
                startDeflect = true;
                dir = player.RelativeMousePos.normalized;
            }
        }
    }
    //public override void OnCollisionEnter2D(PlayerStateMachine player, Collision2D col)
    //{
    //    // we can interrupt once the deflect ends
    //    // so, if we are still in the deflect animation
    //    if (deflectFramesActive)
    //    {
    //        if (col.gameObject == null)
    //        {
    //            return;
    //        }
    //        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Enemy"))
    //        {
    //            Debug.Log("deflect");
    //            EnemyStateMachine esm = col.gameObject.GetComponent<EnemyStateMachine>();
    //            startDeflectHit = true;
    //            // apply player velocity
    //            player.VelocityVector = esm.VelocityVector.normalized * 2.2f;
    //            esm.TakeDamage(0f, esm.VelocityVector.normalized * -1f * 3f);
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
                Debug.Log("Player was hit by enemy in HitState");
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
        player.animator.SetFloat("facing_x", dir.x);
        player.animator.SetFloat("facing_y", dir.y);
        player.animator.SetTrigger("t_deflect");
        player.animator.SetFloat("swordAttack_dir_x", dir.x);
        player.animator.SetFloat("swordAttack_dir_y", dir.y);
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

    public void EventEndDeflect(PlayerStateMachine player)
    {
        player.SwitchState(player.IdleState);
    }

    public void EventAllowBuffer()
    {
        canBufferInput = true;
    }

    public void EventRemoveDeflectFrames()
    {
        deflectFramesActive = false;
    }
}
