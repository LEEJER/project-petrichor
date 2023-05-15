using UnityEngine;
using UnityEngine.InputSystem;
public abstract class PlayerState
{
    public enum NextState
    {
        Nothing,
        Attack,
        Dash,
        Deflect,
        DeflectHit,
        Hit,
        Idle,
        Run,
        Die
    }
    public abstract void EnterState(PlayerStateMachine player);

    public abstract void UpdateState(PlayerStateMachine player);

    public abstract void ExitState(PlayerStateMachine player);

    public abstract void OnMove(PlayerStateMachine player, InputAction.CallbackContext context);
    public abstract void OnFire(PlayerStateMachine player, InputAction.CallbackContext context);
    public abstract void OnDash(PlayerStateMachine player, InputAction.CallbackContext context);
    public abstract void OnDeflect(PlayerStateMachine player, InputAction.CallbackContext context);
    //public abstract void OnCollisionEnter2D(PlayerStateMachine player, Collision2D collision);
    public abstract void OnHitboxEnter(PlayerStateMachine player, Collider2D collision, string selfComponent);
    public abstract void OnHitboxStay(PlayerStateMachine player, Collider2D collision, string selfComponent);
    public abstract void OnHitboxExit(PlayerStateMachine player, Collider2D collision, string selfComponent);
}