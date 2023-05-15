using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeflectState : PlayerState
{
    private bool        canBufferInput  = false;
    private bool        canInterrupt    = false;
    private bool        startDeflect    = false;
    //private bool        startDeflectHit = false;
    public Vector2     dir             = Vector2.zero;
    private NextState   nextState       = NextState.Nothing;

    public bool hasDeflectFrames = false;

    //private Transform   deflectBoxGameObject;
    //private Collider2D deflectBox;
    public override void EnterState(PlayerStateMachine player)
    {
        //deflectBoxGameObject    = player.transform.Find("DeflectBox");
        //deflectBox = deflectBoxGameObject.GetComponent<Collider2D>();
        player.currentState     = PlayerStateMachine.CurrentState.Deflect;
        startDeflect    = true;
        canInterrupt    = true;
        hasDeflectFrames = true;

        //deflectBox.enabled = false;

        //startDeflectHit = false;
        nextState       = NextState.Nothing;
        canBufferInput = true;
        dir = player.RelativeMousePos.normalized;
    }

    public override void UpdateState(PlayerStateMachine player)
    {
        //if (startDeflectHit)
        //{
        //    canInterrupt = false;
        //    startDeflect = false;
        //    canBufferInput = false;
        //    startDeflectHit = false;

        //    player.SwitchState(player.DeflectHitState);

        //}
        //else 
        if (startDeflect && canInterrupt)
        {
            //deflectBox.enabled = true;
            canInterrupt = false;
            startDeflect = false;
            canBufferInput = false;

            //startDeflectHit = false;

            //deflectBoxGameObject.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));
            player.Health += 3f * player.difficultyMultiplier;
            
            Animate(player);

            nextState = NextState.Nothing;
            player.VelocityVector = (player.VelocityVector * 0.75f);
        }
        else if (canInterrupt)
        {
            if (player.Health >= player.MaxHealth)
            {
                nextState = NextState.Die;
            }
            switch (nextState)
            {
                case NextState.DeflectHit:
                    player.SwitchState(player.DeflectHitState);
                    break;
                case NextState.Dash:
                    Interrupt(player, player.DashState);
                    break;
                case NextState.Attack:
                    Interrupt(player, player.AttackState);
                    break;
                case NextState.Hit:
                    Interrupt(player, player.HitState);
                    break;
                case NextState.Die:
                    //Interrupt(player, player.DieState);
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
        //    //player.animator.SetTrigger("t_swordAttack");
        //    Interrupt(player, player.AttackState);
        //}
        //else if (nextState == NextState.Dash && canInterrupt)
        //{
        //    //player.animator.SetTrigger("t_dash");
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
        startDeflect = false;
        canInterrupt = true;
        //startDeflectHit = false;
        nextState = NextState.Nothing;
        canBufferInput = false;
        hasDeflectFrames = false;
        //deflectBox.enabled = false;
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
            if (canBufferInput) { 
                startDeflect = true;
                hasDeflectFrames = true;
                dir = player.RelativeMousePos.normalized;
            }
            // prevent spamming
            if (!canInterrupt)
            {
                hasDeflectFrames = false;
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
                EnemyStateMachine enemy = other.transform.parent.GetComponent<EnemyStateMachine>();
                float deflectAngle = Vector2.Angle(dir, -enemy.LastAttackVector.normalized);
                //Debug.Log(deflectAngle);

                if (hasDeflectFrames && deflectAngle <= 30f)
                {
                    player.VelocityVector = enemy.LastAttackVector.normalized * player.DeflectKnockback * player.SelfKnockback;
                    //startDeflectHit = true;
                    nextState = NextState.DeflectHit;
                    startDeflect = false;
                    canBufferInput = false;
                    canInterrupt = true;

                    player.Health -= 10f;
                    //deflectBox.enabled = true;
                }
                else
                {
                    //deflectBox.enabled = false;
                    player.GetHit(enemy);

                    // interrupt attacks
                    canInterrupt = true;
                    // goto hit state
                    nextState = NextState.Hit;
                }
            }
        }
        // successful deflect
        //else if (selfComponent == "DeflectBox")
        //{
        //    if (other.layer == LayerMask.NameToLayer("Enemy") && other.CompareTag("Hurtbox"))
        //    {
        //        player.VelocityVector = other.transform.parent.GetComponent<EnemyStateMachine>().VelocityVector.normalized * player.DeflectKnockback * player.SelfKnockback;
        //        //startDeflectHit = true;
        //        nextState = NextState.DeflectHit;
        //        startDeflect = false;
        //        canBufferInput = false;
        //        canInterrupt = true;
        //    }
        //}
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
        //deflectBox.enabled = false;
        hasDeflectFrames = false;
    }
}
