using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Events;

public class EnemyStateMachine : StateMachine
{
    public enum CurrentState
    {
        Idle,
        Attack,
        Chase,
        Hit,
        Die,
        Patrol,
        Deflected
    }

    private EnemyState          _currentState;
    public  EnemyIdleState      IdleState       = new EnemyIdleState();
    public  EnemyAttackState    AttackState     = new EnemyAttackState();
    public  EnemyChaseState     ChaseState      = new EnemyChaseState();
    public  EnemyHitState       HitState        = new EnemyHitState();
    public  EnemyDieState       DieState        = new EnemyDieState();
    public  EnemyPatrolState    PatrolState     = new EnemyPatrolState();
    public  EnemyDeflectedState DeflectedState  = new EnemyDeflectedState();

    public CurrentState currentState;

    

    // Stuff for collision detection and movement
    private float               _collisionOffset    = 0.0001f;
    private List<RaycastHit2D>  _castCollisions     = new List<RaycastHit2D>();
    private ContactFilter2D     _movementFilter;

    // Movement params
    //private float _maxVelocity        = 10f;
    private float   _movementSpeed      = 1f;
    private float   _velocityDecayRate  = 8f;

    private float   _chaseSpeed         = 1.1f;
    private float   _attackDistance     = 0.25f;
    private float   _knockbackResistance= 0.75f;
    private float   _knockback          = 2.2f;

    private Vector2 _velocityVector     = Vector2.zero;
    private Vector2 _facingVector       = Vector2.down;
    private Vector2 _lastAttackVector   = Vector2.zero;
    //private Vector2 _desireVector       = Vector2.zero;
    public Vector2[]    DesireVectors   = new Vector2[12];
    public float[]      DesireWeights   = new float[12];
    public float[]      AwayWeights     = new float[12];

    // Game object components
    private Animator    _animator;
    private Collider2D  _enemyCollider;
    private Rigidbody2D _enemyRigidbody;

    // game values
    private float   _health;
    private float   _maxHealth  = 3f;
    private float   _damage = 10f;

    // pathfinding
    private Vector2 _pathfindingTarget;
    private float   _nextWaypointDistance = 0.2f;
    private Path    _path;
    private int     _currentWaypoint;
    private bool    _reachedEndOfPath;
    private Seeker  _seeker;

    public GameManager gameManager;

    public Vector2  FacingVector        { get { return _facingVector; }     set { _facingVector = value; } }
    public Vector2  VelocityVector      { get { return _velocityVector; }   set { _velocityVector = value; } }
    public Vector2  LastAttackVector    { get { return _lastAttackVector; } set { _lastAttackVector = value; } }
    //public Vector2  DesireVector        { get { return _desireVector; }     set { _desireVector = value; } }
    public Animator animator            { get { return _animator; } private set { _animator = value; } }
    public float    MovementSpeed       { get { return _movementSpeed; }    set { _movementSpeed = value; } }
    public float    ChaseSpeed          { get { return _chaseSpeed; }       set { _chaseSpeed = value; } }
    public float    AttackDistance      { get { return _attackDistance; }   set { _attackDistance = value; } }
    public float    KnockbackResistance { get { return _knockbackResistance; }   set { _knockbackResistance = value; } }
    public float    Knockback           { get { return _knockback; }        set { _knockback = value; } }
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
    public float    Damage                  { get { return _damage; }               set { _damage = value; } }
    public Vector2  PathfindingTarget       { get { return _pathfindingTarget; }    set { _pathfindingTarget = value; } }
    public float    NextWaypointDistance    { get { return _nextWaypointDistance; } set { _nextWaypointDistance = value; } }
    public Path     Path                    { get { return _path; }                 set { _path = value; } }
    public int      CurrentWaypoint         { get { return _currentWaypoint; }      set { _currentWaypoint = value; } }
    public bool     ReachedEndOfPath        { get { return _reachedEndOfPath; }     set { _reachedEndOfPath = value; } }
    public Seeker   seeker                  { get { return _seeker; }               set { _seeker = value; } }
    public Rigidbody2D EnemyRigidbody       { get { return _enemyRigidbody; }       set { _enemyRigidbody = value; } }

    private void Start()
    {
        _animator = transform.Find("Sprite").GetComponent<Animator>();
        _enemyCollider = GetComponent<Collider2D>();
        _enemyRigidbody = GetComponent<Rigidbody2D>();

        _movementFilter = new ContactFilter2D();
        _movementFilter.SetLayerMask(LayerMask.GetMask("Environment"));
        _movementFilter.useLayerMask = true;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        _health = _maxHealth;

        _seeker = GetComponent<Seeker>();

        for (int i = 0; i < DesireVectors.Length; i++)
        {
            DesireWeights[i]    = 0;
            AwayWeights[i]      = 0;
            DesireVectors[i]    = new Vector2(Mathf.Cos(2 * i * Mathf.PI / DesireVectors.Length), Mathf.Sin(2 * i * Mathf.PI / DesireVectors.Length)).normalized;
            //Debug.Log(i + ": " + DesireVectors[i]);
        }

        _currentState = IdleState;
        _currentState.EnterState(this);
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

    public void RemoveSelf()
    {
        gameManager.Run_OnEnemyDie();
        Destroy(gameObject);
    }

    public void FindPathTo(Vector2 location)
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(_enemyRigidbody.position, location, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypoint = 0;
        }
    }

    public Vector2 GetChaseVector(Vector2 target, float speed)
    {
        Vector2 targetDirection = UpdateDesireVector(target).normalized;
        //Debug.Log(
        //    AwayWeights[0] + ", " + AwayWeights[1] + ", " +
        //    AwayWeights[2] + ", " + AwayWeights[3] + ", " +
        //    AwayWeights[4] + ", " + AwayWeights[5] + ", " +
        //    AwayWeights[6] + ", " + AwayWeights[7] + ", " +
        //    AwayWeights[8] + ", " + AwayWeights[9] + ", " +
        //    AwayWeights[10] + ", " + AwayWeights[11]
        //    );
        //Vector2 targetDirection = (target - _enemyRigidbody.position).normalized;
        //float targetDistance = Vector2.Distance(target, _enemyRigidbody.position);
        //float desire = Mathf.Min(1f, Mathf.Max(targetDistance - _attackDistance, 0f));
        return targetDirection * speed;
    }

    public Vector2 UpdateDesireVector(Vector2 target)
    {
        Vector2 targetDirection = (target - _enemyRigidbody.position).normalized;
        float targetDistance = Vector2.Distance(target, _enemyRigidbody.position);

        //float desire = Mathf.Min(1f, Mathf.Max(targetDistance - _attackDistance, 0f));
        //return desire;


        for (int i = 0; i < DesireVectors.Length; i++)
        {
            AwayWeights[i] = GetObstructionWeight(DesireVectors[i], 0.25f);
            DesireWeights[i] = Mathf.Max(Vector2.Dot(DesireVectors[i], targetDirection), 0);// * Mathf.Max(targetDistance-_attackDistance, 0.1f);
        }
        int dir = 0;
        float maxWeight = 0f;
        for (int i = 0; i < DesireVectors.Length; i++)
        {
            //float weight = DesireWeights[i] + 4 * AwayWeights[(i + 6) % 12]; 
            float weight = DesireWeights[i] - 3 * AwayWeights[i];
            if (weight > maxWeight)
            {
                maxWeight = weight;
                dir = i;
            }
        }

        return DesireVectors[dir];
    }

    private float GetObstructionWeight(Vector2 direction, float distance)
    {
        float obstructionWeight = 0f;
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Environment") | LayerMask.GetMask("Enemy"));

        RaycastHit2D[] results = new RaycastHit2D[1];
        int count = _enemyCollider.Cast(
            direction,      
            contactFilter,  
            results,        
            distance,       
            true
        );

        //int count = Physics2D.Raycast(_enemyRigidbody.position, direction, contactFilter, results, distance);
        if (count > 0)
        {
            obstructionWeight = 1 - (results[0].distance / distance);
        }
        return obstructionWeight;
    }

    public override void OnHitboxEnter(Collider2D collision, bool isTrigger, string component)
    {
        if (isTrigger)
        {
            _currentState.OnHitboxEnter(this, collision, component);
        }
    }
    public override void OnHitboxStay(Collider2D collision, bool isTrigger, string component)
    {
        if (isTrigger)
        {
            _currentState.OnHitboxStay(this, collision, component);
        }
    }
    public override void OnHitboxExit(Collider2D collision, bool isTrigger, string component)
    {
        if (isTrigger)
        {
            _currentState.OnHitboxExit(this, collision, component);
        }
    }
}
