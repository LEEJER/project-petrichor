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
    private Vector2             relativeMousePos    = Vector2.zero;

    private bool                isMovementLocked    = false;
    private bool                canInterruptCurrentAnimation = true;
    private bool                isTryInterrupt      = false;
    private bool                isInRecovery        = false;

    // Game object components
    private Rigidbody2D         playerRigidBody;
    private Animator            animator;
    private Transform           playerSword;


    // parameters for sword attacks
    private bool                isSwordAttacking        = false;
    private bool                isSwordAttackQueued     = false;
    private int                 swordAttackNumber       = 1;
    private Vector2             swordAttackDirection    = Vector2.zero;

    // parameters for other actions
    private bool                isDeflecting    = false;
    private bool                isDashing       = false;


    private SwordAttackHandler  swordAttackHandler;


    // Start is called before the first frame update
    void Start()
    {
        swordAttackHandler = new SwordAttackHandler();

        inputVector             = Vector2.zero;

        playerRigidBody = GetComponent<Rigidbody2D>();
        animator        = GetComponent<Animator>();
        //spriteRenderer  = GetComponent<SpriteRenderer>();
        playerSword     = transform.Find("Sword");
    }


    private void FixedUpdate()
    {
        // if we have an instance but are not attacking yet
        if (swordAttackHandler.HasRunningInstance() && !isSwordAttacking && canInterruptCurrentAnimation)
        {
            isSwordAttacking = true;
            SwordAttack();
        }
        // always maintain understanding of queue
        isSwordAttackQueued = swordAttackHandler.HasQueuedInstance();
        

        // If there is input and sword is not swinging
        if (inputVector != Vector2.zero && !isMovementLocked) 
        { 
            movementVector = MovePlayer(inputVector); 
            lastInputVector = inputVector;
        } 
        else { movementVector = Vector2.zero; }

        //if (canInterruptCurrentAnimation && inputVector != Vector2.zero)
        //{
        //    isTryInterrupt = true;
        //}
        // Always
        AnimateMovement();
        AnimateSwordAttack();
    }

    private void SwordAttack()
    {
        // update values
        SwordAttackInstance instance = swordAttackHandler.GetRunningInstance();
        swordAttackNumber = instance.attackNum;
        swordAttackDirection = instance.direction;
        // run sword animation
        playerSword.GetComponent<SwordAttack>().AttackWithSword(instance.direction, instance.attackNum);
        // lock movement and interrupt
        isMovementLocked = true;
        canInterruptCurrentAnimation = false;
        // set idle direction
        //lastInputVector = swordAttackDirection;
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
            swordAttackHandler.addSwordAttack(relativeMousePos);
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
        animator.SetFloat("lastMoveX", lastInputVector.x);
        animator.SetFloat("lastMoveY", lastInputVector.y);

        // Set params for Running
        animator.SetFloat("moveX", movementVector.x);
        animator.SetFloat("moveY", movementVector.y);
        animator.SetFloat("moveMagnitude", movementVector.magnitude);
        animator.SetFloat("inputMoveMagnitude", inputVector.magnitude);

        // Set params for interruption
        //animator.SetBool("canInterrupt", canInterruptCurrentAnimation);
        //animator.SetBool("isTryInterrupt", isTryInterrupt);
        //if (isTryInterrupt)
        //{
        //    animator.SetTrigger("interruptTrigger");
        //}
    }

    private void AnimateSwordAttack()
    {
        if (isSwordAttacking)
        {
            setLastRelativeMousePos(swordAttackHandler.GetRunningInstance().direction);
        }

        animator.SetBool("isSwordAttacking", isSwordAttacking);
        animator.SetInteger("swordAttackNum", swordAttackNumber);
        animator.SetBool("isSwordAttackQueued", isSwordAttackQueued);
        animator.SetBool("canInterrupt", canInterruptCurrentAnimation);

        

        // if we are trying another attack
        if (canInterruptCurrentAnimation && isSwordAttackQueued)
        {
            // end the current attack
            swordAttackHandler.RemoveRunningInstance();
            isSwordAttacking = false;
            isSwordAttackQueued = false;
        }
    }

    //// HELPER FUNCTION FOR SWORD ANIMATION
    private void setLastRelativeMousePos(Vector2 dir)
    {
        
        animator.SetFloat("lastRelativeMouseX", dir.x);
        animator.SetFloat("lastRelativeMouseY", dir.y);
        lastInputVector = dir;
    }

    private void EventEndDash()
    {

    }

    private void EventEndSwordAttack()
    {
        swordAttackHandler.RemoveRunningInstance();
        isSwordAttacking = false;
        isSwordAttackQueued = false;
        animator.SetTrigger("recover");
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

    private class SwordAttackHandler
    {
        SwordAttackInstance[] instances;
        private int attackNum = 0;
        private static int maxAttackNum = 2;

        public SwordAttackHandler()
        {
            instances = new SwordAttackInstance[2];
        }

        public void addSwordAttack(Vector2 instanceVector)
        {
            if (instances[1] != null)
            {
                attackNum = (instances[1].attackNum + 1) % maxAttackNum;
            }
            instances[0] = new SwordAttackInstance(instanceVector, attackNum);
            
            if (instances[1] == null)
            {
                instances[1] = instances[0];
                instances[0] = null;
            }
        }

        public SwordAttackInstance GetRunningInstance()
        {
            return instances[1];
        }

        public SwordAttackInstance GetQueuedInstance()
        {
            return instances[0];
        }

        public void RemoveRunningInstance()
        {
            instances[1] = instances[0];
            instances[0] = null;
        }

        public bool HasRunningInstance()
        {
            return (instances[1] != null);
        }

        public bool HasQueuedInstance()
        {
            return (instances[0] != null);
        }
    }

    private class SwordAttackInstance
    {
        public Vector2 direction;
        public int attackNum;

        public SwordAttackInstance(Vector2 dir, int num)
        {
            direction = dir;
            attackNum = num;
        }
    }
}


