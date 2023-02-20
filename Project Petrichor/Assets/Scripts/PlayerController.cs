using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float collisionOffset = 0.01f;
    public ContactFilter2D movementFilter;


    private Vector2     inputVector;
    private Rigidbody2D playerRigidBody;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();




    // Start is called before the first frame update
    void Start()
    {
        inputVector = Vector2.zero;
        playerRigidBody = transform.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // If there is input
        if (inputVector != Vector2.zero)
        {
            // Try to move normally
            if (!TryMove(inputVector))
            {
                // If collision, separate into x and y
                TryMove(new Vector2(inputVector.x, 0));
                TryMove(new Vector2(0, inputVector.y));
            }
        }
    }

    private bool TryMove(Vector2 direction)
    {
        // Check for potential collisions
        int count = playerRigidBody.Cast(
            direction, // X, Y; from -1 to 1. direction from body to look for collisions
            movementFilter, // Settings to determine where collisions can occur (layer)
            castCollisions, // List of collisions to store found collisions after cast is called
            movementSpeed * Time.fixedDeltaTime + collisionOffset // Distance of raycast. Equals movement plus an offset value.
        );
        if (count == 0)
        {
            playerRigidBody.MovePosition(playerRigidBody.position + direction * movementSpeed * Time.fixedDeltaTime);
            return true;
        }
        return false;
    }

    public void PlayerMovementInput(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    /*public void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }*/
}
