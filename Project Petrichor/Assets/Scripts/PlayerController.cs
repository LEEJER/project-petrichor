using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float                movementSpeed = 1f;
    public float                collisionOffset = 0.01f;
    public ContactFilter2D      movementFilter;

    private List<RaycastHit2D>  castCollisions = new List<RaycastHit2D>();
    private Vector2             inputVector;
    private Vector2             lastInputVector = Vector2.down;
    private Vector2             movementVector;
    private Rigidbody2D         playerRigidBody;
    private Animator            animator;
    private SpriteRenderer      spriteRenderer;

    private bool    isAttacking = false;
    private bool    isAttackQueued = false;
    private int     attackNum = 0;
    private static int numberOfAttacks = 2;

    private bool    isDeflecting = false;
    private bool    isDashing = false;


    // Start is called before the first frame update
    void Start()
    {
        inputVector = Vector2.zero;
        playerRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void FixedUpdate()
    {
        // If there is input and sword is not swinging
        if (inputVector != Vector2.zero && !isAttacking) 
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
            // if we are not currently attacking
            if (!isAttacking)
            {
                // attack and clear the queue
                isAttacking = true;
                isAttackQueued = false;
            }
            // if in the middle of an attack
            else if (!isAttackQueued)
            {
                // queue up an attack
                isAttackQueued = true;
                // prepare for the next attack
                attackNum = (attackNum + 1) % numberOfAttacks;
            }
        }
        else if (context.performed) 
        {

        }
        else if (context.canceled) 
        {

        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {

    }

    private void Animate() {
        // sword attacks
        animator.SetBool("isSwordAttacking", isAttacking);
        animator.SetInteger("swordAttackNum", attackNum);
        animator.SetBool("isSwordAttackQueued", isAttackQueued);
        // Flip sprite horizontal where appropriate
        if      (inputVector.x < 0) { spriteRenderer.flipX = true;  } 
        else if (inputVector.x > 0) { spriteRenderer.flipX = false; }

        // Set params for Idle animations
        animator.SetFloat("lastMoveX", lastInputVector.x);
        animator.SetFloat("lastMoveY", lastInputVector.y);

        // Set params for Running
        animator.SetFloat("moveX", movementVector.x);
        animator.SetFloat("moveY", movementVector.y);
        animator.SetFloat("moveMagnitude", movementVector.magnitude);

    
    }

    private void EventEndDash()
    {

    }

    private void EventEndAttack()
    {
        isAttacking = false;
    }

    private void StartAttack()
    {
        // at the start of an attack, set the state and clear the queue
        isAttackQueued = false;
        isAttacking = true;
    }

    private void EventEndDeflect()
    {

    }
}
