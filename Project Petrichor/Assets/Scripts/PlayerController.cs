using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Stuff for collision detection and movement
    public float                maxSpeed            = 1f;
    public float                movementSpeed       = 1f;
    public float                collisionOffset     = 0.08f;
    public ContactFilter2D      movementFilter;
    private List<RaycastHit2D>  castCollisions      = new List<RaycastHit2D>();
    private Vector2             inputVector;
    private Vector2             lastInputVector     = Vector2.down;
    private Vector2             movementVector;
    private Vector2             relativeMousePos    = Vector2.zero;
    private Vector2             velocityVector      = Vector2.zero;
    private float               velocityDecayRate   = 4f;

    private bool                isMovementLocked    = false;
    private bool                canInterruptCurrentAnimation = true;

    // Game object components
    private Rigidbody2D         playerRigidBody;
    private Animator            animator;
    private Sword               sword;

    // Start is called before the first frame update
    void Start()
    {
        inputVector     = Vector2.zero;

        playerRigidBody = GetComponent<Rigidbody2D>();
        animator        = GetComponent<Animator>();
        sword           = new Sword(transform);
    }


    private void FixedUpdate()
    {
        // handle old velocity first
        velocityVector += (Vector2.zero - velocityVector.normalized) * Mathf.Min(velocityDecayRate * Time.fixedDeltaTime, velocityVector.magnitude);
        if (velocityVector.magnitude < 0.01f) { velocityVector = Vector2.zero; }
        MovePlayer(velocityVector.normalized, velocityVector.magnitude);

        // If there is input and sword is not swinging
        if (inputVector != Vector2.zero && !isMovementLocked && canInterruptCurrentAnimation) 
        {
            //movementVector = MovePlayer(inputVector, movementSpeed); 
            velocityVector = inputVector * movementSpeed;
            lastInputVector = inputVector;
        }

        if (sword.isAttacking)
        {
            isMovementLocked = true;
            //velocityVector = Vector2.zero;
            lastInputVector = sword.dir;
        }

        AnimateMovement();
        AnimateSwordAttack();
    }

    private Vector2 MovePlayer(Vector2 direction, float speed)
    {
        Vector2 newDir = new Vector2(0, 0);
        // Check for potential collisions
        int count = RigidbodyRaycast(direction, speed);
        
        if (count == 0)
        {
            // If there were no collisions, continue as normal
            newDir = direction * speed * Time.fixedDeltaTime;
        } 
        else 
        {
            // There was a collision, try each direction
            int countX = RigidbodyRaycast(new Vector2(direction.x, 0), (Mathf.Abs(direction.x) * speed));
            int countY = RigidbodyRaycast(new Vector2(0, direction.y), (Mathf.Abs(direction.y) * speed));
            if (countX == 0) 
            {
                newDir = new Vector2(direction.x, 0) * (Mathf.Abs(direction.x) * speed) * Time.fixedDeltaTime;
            } 
            else if (countY == 0) 
            {
                newDir = new Vector2(0, direction.y) * (Mathf.Abs(direction.y) * speed) * Time.fixedDeltaTime;
            }
        }
        playerRigidBody.MovePosition(playerRigidBody.position + newDir);
        return newDir;
    }
    
    // THIS IS A HELPER FUNCTION TO: MovePlayer()
    private int RigidbodyRaycast(Vector2 direction, float speed) 
    {
        return playerRigidBody.Cast(
            direction, // X, Y; from -1 to 1. direction from body to look for collisions
            movementFilter, // Settings to determine where collisions can occur (layer)
            castCollisions, // List of collisions to store found collisions after cast is called
            speed * Time.fixedDeltaTime + collisionOffset // Distance of raycast. Equals movement plus an offset value.
        );
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context) 
    {
        if (context.started) 
        {
            // if we can interrupt, start sword attack
            if (canInterruptCurrentAnimation)
            {
                bool attackExists = sword.TryAddAttack(relativeMousePos);
                // completely new attack
                if (!attackExists) {
                    canInterruptCurrentAnimation = false;
                }
            }
        }
        else if (context.performed) { }
        else if (context.canceled) { }
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        relativeMousePos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()) - transform.position;
    }

    public void OnDash(InputAction.CallbackContext context)
    {

    }

    private void AnimateMovement() {
        // Set params for Idle animations
        animator.SetFloat("facing_x", lastInputVector.x);
        animator.SetFloat("facing_y", lastInputVector.y);

        // Set params for Running
        animator.SetFloat("velocity_x", velocityVector.x);
        animator.SetFloat("velocity_y", velocityVector.y);
        animator.SetFloat("velocity_magnitude", velocityVector.magnitude);
    }

    private void AnimateSwordAttack()
    {
        animator.SetBool("canInterrupt", canInterruptCurrentAnimation);
        // attack just started
        if (sword.isAttacking)
        {
            sword.isAttacking = false;
            animator.SetTrigger("t_swordAttack");
            animator.SetInteger("swordAttack_num", sword.num);
            animator.SetFloat("swordAttack_dir_x", sword.dir.x);
            animator.SetFloat("swordAttack_dir_y", sword.dir.y);
            canInterruptCurrentAnimation = false;

            sword.SwordAttack();
            velocityVector = (velocityVector * 0.5f) + sword.dir.normalized * sword.swingMovementSpeed;
        }
    }

    private void EventEndDash()
    {

    }

    private void EventEndSwordAttack()
    {
        EventUnlockMovement();
    }

    private void StartSwordAttack()
    {
        
    }

    private void EventEndDeflect()
    {

    }

    private void EventLockMovement()
    {
        isMovementLocked = true;
    }

    private void EventUnlockMovement()
    {
        isMovementLocked = false;
    }

    private void EventAllowInterruptAnimation()
    {
        canInterruptCurrentAnimation = true;
    }

    private void EventDisableInterruptAnimation()
    {

    }

    private class Sword
    {
        public bool                 isAttacking = false;
        public int                  num = 0;
        public Vector2              dir = Vector2.zero;
        public Transform            playerSword;
        public float                swingMovementSpeed = 1.45f;

        public Sword(Transform transform)
        {
            playerSword = transform.Find("Sword");
        }

        // returns true if an attack already exists
        public bool TryAddAttack(Vector2 dir)
        {
            if (this.dir != Vector2.zero)
            {
                AddAttack(dir);
                return true;
            }
            else
            {
                AddAttack(dir);
                return false;
            }
            
        }

        private void AddAttack(Vector2 dir)
        {
            this.dir = dir;
            num = (num + 1) % 2;
            isAttacking = true;
        }

        public void SwordAttack()
        {
            playerSword.GetComponent<SwordAttack>().AttackWithSword(dir, num);
        }
    }
}


