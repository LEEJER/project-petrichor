using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    // STATE MACHINE STUFF
    PlayerState                 currentState;
    public PlayerAttackState    AttackState     = new PlayerAttackState();
    public PlayerDashState      DashState       = new PlayerDashState();
    public PlayerIdleState      IdleState       = new PlayerIdleState();
    public PlayerRunState       RunState        = new PlayerRunState();
    public PlayerDeflectState   DeflectState    = new PlayerDeflectState();
    // STATE MACHINE STUFF END

    // Stuff for collision detection and movement
    private float               _collisionOffset    = 0.0001f;
    private ContactFilter2D     _movementFilter;
    private List<RaycastHit2D>  _castCollisions     = new List<RaycastHit2D>();

    // Movement params
    public float MaxVelocity        = 10f;
    public float MovementSpeed      = 1f;
    public float VelocityDecayRate  = 4.5f;
    public float DashSpeed          = 3f;

    Vector2 _velocityVector     = Vector2.zero;
    Vector2 _inputVector        = Vector2.zero;
    Vector2 _relativeMousePos   = Vector2.zero;
    Vector2 _facingVector       = Vector2.down;

    public Vector2 FacingVector     { get { return _facingVector; }     set { _facingVector = value; } }
    public Vector2 RelativeMousePos { get { return _relativeMousePos; } set { _relativeMousePos = value; } }
    public Vector2 InputVector      { get { return _inputVector; }      set { _inputVector = value; } }
    public Vector2 VelocityVector   { get { return _velocityVector; }   set { _velocityVector = value; } }


    // general animation
    bool isMovementLocked = false;
    bool canInterruptCurrentAnimation = true;

    // avility animation
    bool isDashing = false;
    Sword _sword;

    // Game object components
    Rigidbody2D playerRigidBody;
    private Animator _animator;

    public Animator animator    { get { return _animator; } private set { _animator = value; } }
    public Sword sword          { get { return _sword; }    private set { _sword = value; } }


    // Start is called before the first frame update
    void Start()
    {
        // Initial state is IDLE
        currentState = IdleState;
        currentState.EnterState(this);

        // setup other
        playerRigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sword = transform.Find("Sword").GetComponent<Sword>();
    }

    private void FixedUpdate()
    {
        MovePlayerBasedOnVelocity();
        currentState.UpdateState(this);
    }

    private void MovePlayerBasedOnVelocity()
    {
        // Velocity decay
        _velocityVector += (Vector2.zero - _velocityVector.normalized) * Mathf.Min(VelocityDecayRate * Time.fixedDeltaTime, _velocityVector.magnitude);
        if (_velocityVector.magnitude < 0.01f) { _velocityVector = Vector2.zero; }
        // Actually move
        MovePlayer(_velocityVector.normalized, _velocityVector.magnitude);
    }

    private Vector2 MovePlayer(Vector2 direction, float speed)
    {
        Vector2 newDir = new Vector2(0, 0);
        // Check for potential collisions
        int count = RigidbodyRaycast(direction, speed);

        // If there were no collisions, continue as normal
        if (count == 0) { newDir = direction * speed * Time.fixedDeltaTime; }
        else
        {
            // There was a collision, try each direction
            int countX = RigidbodyRaycast(new Vector2(direction.x, 0), (Mathf.Abs(direction.x) * speed));
            int countY = RigidbodyRaycast(new Vector2(0, direction.y), (Mathf.Abs(direction.y) * speed));
            // only move in the direction where there is no collision
            if (countX == 0) { newDir = new Vector2(direction.x, 0) * (Mathf.Abs(direction.x) * speed) * Time.fixedDeltaTime; }
            else if (countY == 0) { newDir = new Vector2(0, direction.y) * (Mathf.Abs(direction.y) * speed) * Time.fixedDeltaTime; }
        }
        playerRigidBody.MovePosition(playerRigidBody.position + newDir);
        return newDir;
    }

    private int RigidbodyRaycast(Vector2 direction, float speed)
    {
        return playerRigidBody.Cast(
            direction, // X, Y; from -1 to 1. direction from body to look for collisions
            _movementFilter, // Settings to determine where collisions can occur (layer)
            _castCollisions, // List of collisions to store found collisions after cast is called
            speed * Time.fixedDeltaTime + _collisionOffset // Distance of raycast. Equals movement plus an offset value.
        );
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _inputVector = context.ReadValue<Vector2>();
        currentState.OnMove(this, context);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        currentState.OnFire(this, context);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        currentState.OnDash(this, context);
    }

    public void OnDeflect(InputAction.CallbackContext context)
    {
        currentState.OnDeflect(this, context);
    }

    public void AnimateFloat(string param, float val)
    {
        animator.SetFloat(param, val);
    }
    public void OnMouse(InputAction.CallbackContext context)
    {
        RelativeMousePos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()) - transform.position;
    }

    public void SwitchState(PlayerState state)
    {
        currentState.ExitState(this);
        currentState = state;
        state.EnterState(this);
    }

    public void Event_AttackState_AllowInterrupt()
    {
        AttackState.EventAllowInterrupt();
    }

    public void Event_AttackState_EndSwordAttack()
    {
        AttackState.EventEndSwordAttack();
    }

    public void Event_AttackState_AllowBuffer()
    {
        AttackState.EventAllowBuffer();
    }
}
