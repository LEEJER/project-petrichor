using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeflectState : PlayerState
{
    private bool        canBufferInput  = false;
    private bool        canInterrupt    = false;
    private bool        startDeflect    = false;
    private bool        startDeflectHit = false;
    private Vector2     dir             = Vector2.zero;
    private NextState   startNext       = NextState.Nothing;

    private Transform   deflectBoxGameObject;
    private Collider2D  deflectBox;
    public override void EnterState(PlayerStateMachine player)
    {
        deflectBoxGameObject    = player.transform.Find("DeflectBox");
        deflectBox              = deflectBoxGameObject.GetComponent<Collider2D>();
        player.currentState     = PlayerStateMachine.CurrentState.Deflect;
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
            deflectBox.enabled = true;
            canInterrupt = false;
            startDeflect = false;
            canBufferInput = false;
            //startDeflectHit = false;

            deflectBoxGameObject.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));

            
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
        canBufferInput = false;
        deflectBox.enabled = false;
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

    public override void OnHitboxEnter(PlayerStateMachine player, Collider2D collision, string selfComponent)
    {
        GameObject other = collision.gameObject;
        if (selfComponent == "Hitbox")
        {
            // if our hitbox was hit by enemy hurtbox
            if (other.layer == LayerMask.NameToLayer("Enemy") && other.CompareTag("Hurtbox"))
            {
                deflectBox.enabled = false;
                // take damage
                // apply self knockback
                // interrupt attacks
                // goto hit state
            }
        }
        // successful deflect
        else if (selfComponent == "DeflectBox")
        {
            if (other.layer == LayerMask.NameToLayer("Enemy") && other.CompareTag("Hurtbox"))
            {
                player.VelocityVector = other.transform.parent.GetComponent<EnemyStateMachine>().VelocityVector.normalized * player.DeflectKnockback * player.SelfKnockback;
                startDeflectHit = true;
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
        deflectBox.enabled = false;
    }
}
