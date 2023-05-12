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
        Idle,
        Run
    }
    public abstract void EnterState(PlayerStateMachine player);

    public abstract void UpdateState(PlayerStateMachine player);

    public abstract void ExitState(PlayerStateMachine player);

    public abstract void OnMove(PlayerStateMachine player, InputAction.CallbackContext context);
    public abstract void OnFire(PlayerStateMachine player, InputAction.CallbackContext context);
    public abstract void OnDash(PlayerStateMachine player, InputAction.CallbackContext context);
    public abstract void OnDeflect(PlayerStateMachine player, InputAction.CallbackContext context);
    public abstract void OnCollisionEnter2D(PlayerStateMachine player, Collision2D col);
}
