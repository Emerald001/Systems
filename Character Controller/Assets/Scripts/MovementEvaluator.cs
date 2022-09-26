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

    public bool CanGrabLedge() {
        if (owner.velocity.y > 1 && owner.velocity.y < 5)
            return false;

        Ray RayOne = new(owner.controller.transform.position + new Vector3(0, 1.5f, 0), owner.transform.forward);
        Ray RayTwo = new(owner.controller.transform.position + new Vector3(0, 1.7f, 0), owner.transform.forward);

        if (Physics.Raycast(RayOne, out var hitOne, .6f, owner.EdgeLayer)) {
            if (!Physics.Raycast(RayTwo, out var hitTwo, .6f, owner.EdgeLayer)) {
                return true;
            }
        }

        return false;
    }
}