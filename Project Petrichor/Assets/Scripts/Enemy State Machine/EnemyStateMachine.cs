using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private EnemyState          _currentState;
    public  EnemyIdleState      IdleState       = new EnemyIdleState();
    public  EnemyAttackState    AttackState     = new EnemyAttackState();
    public  EnemyChaseState     ChaseState      = new EnemyChaseState();
    public  EnemyHitState       HitState        = new EnemyHitState();
    public  EnemyDieState       DieState        = new EnemyDieState();

    // Stuff for collision detection and movement
    private float               _collisionOffset    = 0.0001f;
    private List<RaycastHit2D>  _castCollisions     = new List<RaycastHit2D>();
    private ContactFilter2D     _movementFilter;

    // Movement params
    //private float _maxVelocity        = 10f;
    private float _movementSpeed        = 1f;
    private float _velocityDecayRate    = 8f;

    Vector2 _velocityVector = Vector2.zero;
    Vector2 _facingVector   = Vector2.down;

    // Game object components
    private Animator        _animator;
    private Collider2D      _enemyCollider;
    private Rigidbody2D     _enemyRigidbody;

    // game values
    private float _health;
    private float _maxHealth = 3f;

    public Vector2  FacingVector { get { return _facingVector; } set { _facingVector = value; } }
    public Vector2  VelocityVector { get { return _velocityVector; } set { _velocityVector = value; } }
    public Animator animator { get { return _animator; } private set { _animator = value; } }
    public float    MovementSpeed { get { return _movementSpeed; } set { _movementSpeed = value; } }
    public float    Health
    {
        get { return _health; }
        set
        {
            _health = value;
            if (_health > _maxHealth) { _health = _maxHealth; }
            else if (_health <= 0) { _health = 0; }
        }
    }

    private void Start()
    {
        _currentState = IdleState;
        _currentState.EnterState(this);

        _animator = transform.Find("Sprite").GetComponent<Animator>();
        _enemyCollider = GetComponent<Collider2D>();
        _enemyRigidbody = GetComponent<Rigidbody2D>();

        _movementFilter = new ContactFilter2D();
        _movementFilter.SetLayerMask(LayerMask.GetMask("Environment"));
        _movementFilter.useLayerMask = true;

        _health = _maxHealth;
    }

    private void FixedUpdate()
    {
        _currentState.UpdateState(this);
        MoveBasedOnVelocity();
    }

    private void MoveBasedOnVelocity()
    {
        // Actually move
        Move(_velocityVector.normalized, _velocityVector.magnitude);
        // Velocity decay
        _velocityVector += (Vector2.zero - _velocityVector.normalized) * Mathf.Min(_velocityDecayRate * Time.fixedDeltaTime, _velocityVector.magnitude);
        if (_velocityVector.magnitude < 0.01f) { _velocityVector = Vector2.zero; }
    }

    private Vector2 Move(Vector2 direction, float speed)
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
        _enemyRigidbody.MovePosition(_enemyRigidbody.position + newDir);
        return newDir;
    }

    private int RigidbodyRaycast(Vector2 direction, float speed)
    {
        return _enemyCollider.Cast(
            direction, // X, Y; from -1 to 1. direction from body to look for collisions
            _movementFilter, // Settings to determine where collisions can occur (layer)
            _castCollisions, // List of collisions to store found collisions after cast is called
            speed * Time.fixedDeltaTime + _collisionOffset, // Distance of raycast. Equals movement plus an offset value.
            true
        );
    }

    public void SwitchState(EnemyState state)
    {
        _currentState.ExitState(this);
        _currentState = state;
        _currentState.EnterState(this);
    }


    public bool TakeDamage(float damage, Vector2 push)
    {
        bool hit = !_currentState.Equals(HitState);
        _currentState.OnTakeDamage(this, damage, push);
        return hit;
    }

    public void RemoveSelf()
    {
        Destroy(gameObject);
    }

    public void OnDetectionBoxEnter(Collider2D collision)
    {
        _currentState.OnDetectionBoxEnter(this, collision);
    }

    public void OnDetectionBoxStay(Collider2D collision)
    {
        _currentState.OnDetectionBoxStay(this, collision);
    }
}
