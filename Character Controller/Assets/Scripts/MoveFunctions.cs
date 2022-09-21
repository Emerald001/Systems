using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveFunctions : MonoBehaviour
{
    public float jumpHeight;
    public float moveSpeed;
    public float airSpeed;

    private MovementManager manager;

    private void Start() {
        manager = GetComponent<MovementManager>();
    }

    public void Move(InputAction.CallbackContext context) {
        var input = context.ReadValue<Vector2>();

        manager.velocity += (Vector3)input * moveSpeed;
    }

    public void Jump() {
        manager.velocity.y += jumpHeight;
    }
}