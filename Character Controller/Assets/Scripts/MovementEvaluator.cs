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

    public GameObject CanGrabLedge() {
        if (owner.velocity.y > 1 && owner.velocity.y < 5)
            return null;

        Ray rayOne = new(owner.controller.transform.position + new Vector3(0, 2.2f, 0), owner.transform.forward);
        Ray rayTwo = new(owner.controller.transform.position + new Vector3(0, 2.4f, 0), owner.transform.forward);

        if (Physics.Raycast(rayOne, out var hitOne, .6f, owner.EdgeLayer)) {
            if (!Physics.Raycast(rayTwo, out var hitTwo, .6f, owner.EdgeLayer)) {
                return hitOne.collider.gameObject;
            }
        }

        return null;
    }

    public GameObject CanGrabLedge(Vector3 Dir) {
        Ray ray = new(owner.transform.position + (new Vector3(0, 2.2f, 0) + owner.transform.forward * .6f), Dir);

        Debug.DrawRay(owner.transform.position + (new Vector3(0, 2.2f, 0) + owner.transform.forward * .6f), Dir);

        if(Physics.Raycast(ray, out var hit, owner.climbDistance, owner.EdgeLayer)) {
            return hit.collider.gameObject;
        }
        return null;
    }

    public GameObject CanGrabLedgeLeap(Vector3 Dir) {
        Ray ray = new(owner.transform.position + (new Vector3(0, 2.2f, 0) + owner.transform.forward * .6f), Dir);

        if (Physics.Raycast(ray, out var hit, owner.leapClimbDistance, owner.EdgeLayer)) {
            return hit.collider.gameObject;
        }
        return null;
    }

    public bool CanGoOntoLedge() {
        Ray ray = new(owner.controller.transform.position + new Vector3(0, 2.4f, 0), owner.transform.forward);

        if (!Physics.Raycast(ray, out var hit, 3f)) {
            return true;
        }

        return false;
    }
}