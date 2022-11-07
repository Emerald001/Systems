using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask Ground;
    bool Grounded;

    public Transform orientation;

    Vector3 moveDir;

    Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate() {
        Vector2 input = new(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"));

        moveDir = orientation.forward * input.x + orientation.right * input.y;

        rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
    }


}