using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEvaluator
{
    public MovementManager owner;

    public Vector3 GetSlopeNormal() {
        Vector3 origin = owner.controller.transform.position + new Vector3(0, .1f, 0);

        Physics.Raycast(origin, Vector3.down, out var hit, 1f);
        return IsGrounded() ? hit.normal : Vector3.up;
    }

    public bool IsGrounded() {
        if (Physics.CheckSphere(owner.transform.position + new Vector3(0, .2f, 0), .5f, owner.GroundLayer)) {
            return true;
        }

        return false;
    }

    public bool TouchedRoof() {
        if (Physics.CheckSphere(owner.transform.position + new Vector3(0, 1.6f, 0), .5f, owner.GroundLayer)) {
            return true;
        }

        return false;
    }
}