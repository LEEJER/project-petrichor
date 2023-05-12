using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    // STATE MACHINE STUFF
    private PlayerState             _currentState;
    public  PlayerAttackState       AttackState     = new PlayerAttackState();
    public  PlayerDashState         DashState       = new PlayerDashState();
    public  PlayerIdleState         IdleState       = new PlayerIdleState();
    public  PlayerRunState          RunState        = new PlayerRunState();
    public  PlayerDeflectState      DeflectState    = new PlayerDeflectState();
    public  PlayerDeflectHitState   DeflectHitState = new PlayerDeflectHitState();
    // STATE MACHINE STUFF END

    // Stuff for collision detection and movement
    private float               _collisionOffset    = 0.0001f;
    private List<RaycastHit2D>  _castCollisions     = new List<RaycastHit2D>();
    private ContactFilter2D     _movementFilter;

    // Movement params
    //private float _maxVelocity        = 10f;
    private float _movementSpeed      = 1f;
    private float _velocityDecayRate  = 5f;
    private float _dashSpeed          = 3f;

    Vector2 _velocityVector     = Vector2.zero;
    Vector2 _inputVector        = Vector2.zero;
    Vector2 _relativeMousePos   = Vector2.zero;
    Vector2 _facingVector       = Vector2.down;

    // Game object components
    private Animator    _animator;
    private Sword       _sword;
    private Rigidbody2D _playerRigidBody;

    public Vector2  FacingVector        { get { return _facingVector; }     set { _facingVector = value; } }
    public Vector2  RelativeMousePos    { get { return _relativeMousePos; } set { _relativeMousePos = value; } }
    public Vector2  InputVector         { get { return _inputVector; }      set { _inputVector = value; } }
    public Vector2  VelocityVector      { get { return _velocityVector; }   set { _velocityVector = value; } }
    public Animator animator            { get { return _animator; } private set { _animator = value; } }
    public Sword    sword               { get { return _sword; }    private set { _sword = value; } }
    public float    MovementSpeed       { get { return _movementSpeed; }    set { _movementSpeed = value; } }
    public float    DashSpeed           { get { return _dashSpeed; }        set { _dashSpeed = value; } }
    //public float    MaxVelocity         { get { return _maxVelocity; }      set { _maxVelocity = value; } }


    // Start is called before the first frame update
    void Start()
    {
        // Initial state is IDLE
        _currentState = IdleState;
        _currentState.EnterState(this);

        // setup other
        _animator           = transform.Find("Sprite").GetComponent<Animator>();
        _playerRigidBody    = GetComponent<Rigidbody2D>();
        _sword              = transform.Find("Sword").GetComponent<Sword>();
    }

    private void FixedUpdate()
    {
        MovePlayerBasedOnVelocity();
        _currentState.UpdateState(this);
    }

    private void MovePlayerBasedOnVelocity()
    {
        // Velocity decay
        _velocityVector += (Vector2.zero - _velocityVector.normalized) * Mathf.Min(_velocityDecayRate * Time.fixedDeltaTime, _velocityVector.magnitude);
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
        _playerRigidBody.MovePosition(_playerRigidBody.position + newDir);
        return newDir;
    }

    private int RigidbodyRaycast(Vector2 direction, float speed)
    {
        return _playerRigidBody.Cast(
            direction, // X, Y; from -1 to 1. direction from body to look for collisions
            _movementFilter, // Settings to determine where collisions can occur (layer)
            _castCollisions, // List of collisions to store found collisions after cast is called
            speed * Time.fixedDeltaTime + _collisionOffset // Distance of raycast. Equals movement plus an offset value.
        );
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _inputVector = context.ReadValue<Vector2>();
        _currentState.OnMove(this, context);
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        _currentState.OnFire(this, context);
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        _currentState.OnDash(this, context);
    }
    public void OnDeflect(InputAction.CallbackContext context)
    {
        _currentState.OnDeflect(this, context);
    }
    public void OnMouse(InputAction.CallbackContext context)
    {
        RelativeMousePos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()) - transform.position;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        _currentState.OnCollisionEnter2D(this, collision);
    }
    
    
    public void SwitchState(PlayerState state)
    {
        _currentState.ExitState(this);
        _currentState = state;
        state.EnterState(this);
    }

    public void Event_AttackState_AllowInterrupt()      { AttackState.EventAllowInterrupt(); }
    public void Event_AttackState_EndSwordAttack()      { AttackState.EventEndSwordAttack(this); }
    public void Event_AttackState_AllowBuffer()         { AttackState.EventAllowBuffer(); }
    public void Event_DashState_AllowInterrupt()        { DashState.EventAllowInterrupt(); }
    public void Event_DashState_AllowNewDash()          { DashState.EventAllowNewDash(); }
    public void Event_DashState_EndDash()               { DashState.EventEndDash(this); }
    public void Event_DeflectState_AllowInterrupt()     { DeflectState.EventAllowInterrupt(); }
    public void Event_DeflectState_AllowBuffer()        { DeflectState.EventAllowBuffer(); }
    public void Event_DeflectState_EndDeflect()         { DeflectState.EventEndDeflect(this); }
    public void Event_DeflectState_RemoveDeflectFrames(){ DeflectState.EventRemoveDeflectFrames(); }
    public void Event_DeflectHitState_AllowInterrupt()  { DeflectHitState.EventAllowInterrupt(); }
    public void Event_DeflectHitState_AllowBuffer()     { DeflectHitState.EventAllowBuffer(); }
    public void Event_DeflectHitState_EndDeflectHit()   { DeflectHitState.EventEndDeflectHit(this); }
}
