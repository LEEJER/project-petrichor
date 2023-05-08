using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Stuff for collision detection and movement
    public float                movementSpeed       = 1f;
    public float                collisionOffset     = 0.01f;
    public ContactFilter2D      movementFilter;
    private List<RaycastHit2D>  castCollisions      = new List<RaycastHit2D>();
    private Vector2             inputVector;
    private Vector2             lastInputVector     = Vector2.down;
    private Vector2             movementVector;


    private bool                isMovementLocked    = false;
    private bool                canInterruptCurrentAnimation = false;

    // Game object components
    private Rigidbody2D         playerRigidBody;
    private Animator            animator;
    private Transform           playerSword;
    //private SpriteRenderer      spriteRenderer;

    // direction vectors for sword attacks
    private Vector2             relativeMousePos;
    private Vector2             currentlyRunningSwordAttackVector;
    private Vector2             queuedSwordAttackVector;

    // parameters for sword attacks
    private bool                isSwordAttacking        = false;
    private bool                isSwordAttackQueued     = false;
    private int                 swordAttackNumber       = 1;
    private static int          maxNumberOfSwordAttacks = 2;

    // parameters for other actions
    private bool                isDeflecting    = false;
    private bool                isDashing       = false;


    // Start is called before the first frame update
    void Start()
    {
        inputVector             = Vector2.zero;
        relativeMousePos        = Vector2.zero;

        currentlyRunningSwordAttackVector = Vector2.zero;
        queuedSwordAttackVector           = Vector2.zero;

        playerRigidBody = GetComponent<Rigidbody2D>();
        animator        = GetComponent<Animator>();
        //spriteRenderer  = GetComponent<SpriteRenderer>();
        playerSword     = transform.Find("Sword");
    }


    private void FixedUpdate()
    {
        // if there is an item in the queue, attack is queued
        if (queuedSwordAttackVector != Vector2.zero) { isSwordAttackQueued = true; }
        // If we can move the queued attack to the run, do so
        if (isSwordAttackQueued && currentlyRunningSwordAttackVector == Vector2.zero)
        {
            currentlyRunningSwordAttackVector = queuedSwordAttackVector;
            queuedSwordAttackVector = Vector2.zero;
            // cycle animation variants
            swordAttackNumber = (swordAttackNumber + 1) % maxNumberOfSwordAttacks;
            // the queue bool will be cleared at the start of the animation
        }

        // If there is input and sword is not swinging
        if (inputVector != Vector2.zero && !isMovementLocked) 
        { 
            movementVector = MovePlayer(inputVector); 
            lastInputVector = inputVector;
        } 
        else { movementVector = Vector2.zero; }
        // Always
        Animate();
    }

    private Vector2 MovePlayer(Vector2 direction)
    {
        Vector2 newDir = new Vector2(0, 0);
        // Check for potential collisions
        int count = RigidbodyRaycast(direction);
        
        if (count == 0)
        {
            // If there were no collisions, continue as normal
            newDir = direction * movementSpeed * Time.fixedDeltaTime;
        } 
        else 
        {
            // There was a collision, try each direction
            int countX = RigidbodyRaycast(new Vector2(direction.x, 0));
            int countY = RigidbodyRaycast(new Vector2(0, direction.y));
            if (countX == 0) {
                newDir = new Vector2(direction.x, 0) * movementSpeed * Time.fixedDeltaTime;
            } else if (countY == 0) {
                newDir = new Vector2(0, direction.y) * movementSpeed * Time.fixedDeltaTime;
            }
        }
        playerRigidBody.MovePosition(playerRigidBody.position + newDir);
        return newDir;
    }
    
    // THIS IS A HELPER FUNCTION TO: MovePlayer()
    private int RigidbodyRaycast(Vector2 direction) 
    {
        return playerRigidBody.Cast(
            direction, // X, Y; from -1 to 1. direction from body to look for collisions
            movementFilter, // Settings to determine where collisions can occur (layer)
            castCollisions, // List of collisions to store found collisions after cast is called
            movementSpeed * Time.fixedDeltaTime + collisionOffset // Distance of raycast. Equals movement plus an offset value.
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
            queuedSwordAttackVector = relativeMousePos;
        }
        else if (context.performed) 
        {

        }
        else if (context.canceled) 
        {

        }
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        relativeMousePos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()) - transform.position;
    }

    public void OnDash(InputAction.CallbackContext context)
    {

    }

    private void Animate() {
        // Set direction for sword attack
        if (currentlyRunningSwordAttackVector != Vector2.zero)
        {
            isSwordAttacking = true;
            setLastRelativeMousePos();
        }
        if (isSwordAttackQueued)
        {
            setLastRelativeMousePos();
        }
        // Set params for sword attack
        animator.SetBool("isSwordAttacking", isSwordAttacking);
        animator.SetInteger("swordAttackNum", swordAttackNumber);
        animator.SetBool("isSwordAttackQueued", isSwordAttackQueued);

        // Set params for Idle animations
        animator.SetFloat("lastMoveX", lastInputVector.x);
        animator.SetFloat("lastMoveY", lastInputVector.y);

        // Set params for Running
        animator.SetFloat("moveX", movementVector.x);
        animator.SetFloat("moveY", movementVector.y);
        animator.SetFloat("moveMagnitude", movementVector.magnitude);
        animator.SetFloat("inputMoveMagnitude", inputVector.magnitude);

        // Set params for interruption
        animator.SetBool("canInterrupt", canInterruptCurrentAnimation);
    }

    // HELPER FUNCTION FOR SWORD ANIMATION
    private void setLastRelativeMousePos()
    {
        animator.SetFloat("lastRelativeMouseX", currentlyRunningSwordAttackVector.x);
        animator.SetFloat("lastRelativeMouseY", currentlyRunningSwordAttackVector.y);
        lastInputVector = currentlyRunningSwordAttackVector;
    }

    private void EventEndDash()
    {

    }

    private void EventEndSwordAttack()
    {
        isSwordAttacking = false;
        currentlyRunningSwordAttackVector = Vector2.zero;
    }

    private void StartSwordAttack()
    {
        // at the start of an attack, set the state and clear the queue
        EventLockMovement();
        EventDisableInterruptAnimation();
        isSwordAttackQueued = false;
        playerSword.GetComponent<SwordAttack>().AttackWithSword(currentlyRunningSwordAttackVector, swordAttackNumber, isSwordAttacking);
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
        canInterruptCurrentAnimation = false;
    }
}
