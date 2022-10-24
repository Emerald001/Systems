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

        Ray rayOne = new(owner.controller.transform.position + new Vector3(0, 1.6f, 0), owner.transform.forward);
        Ray rayTwo = new(owner.controller.transform.position + new Vector3(0, 1.65f, 0), owner.transform.forward);

        if (Physics.Raycast(rayOne, out var hitOne, .6f, owner.EdgeLayer)) {
            if (!Physics.Raycast(rayTwo, out var hitTwo, .6f, owner.EdgeLayer)) {
                owner.CurrentLedge = hitOne.collider.gameObject;
                return hitOne.collider.gameObject;
            }
        }

        return null;
    }

    public GameObject CanGrabLedge(Vector3 Dir) {
        Vector3 pos = Vector3.zero;

        if(Dir == Vector3.up)
            pos = new Vector3(owner.transform.position.x, owner.CurrentLedge.transform.position.y + .05f, owner.transform.position.z) + owner.transform.forward * .6f;
        else if(Dir == Vector3.down)
            pos = new Vector3(owner.transform.position.x, owner.CurrentLedge.transform.position.y - .05f, owner.transform.position.z) + owner.transform.forward * .6f;

        Ray ray = new(pos, Dir);

        Debug.DrawRay(pos, Dir);

        if(Physics.Raycast(ray, out var hit, owner.climbDistance, owner.EdgeLayer)) {
            if (hit.collider.gameObject == owner.CurrentLedge) {
                Debug.LogError("Same Edge");
                return null;
            }

            owner.CurrentLedge = hit.collider.gameObject;
            return hit.collider.gameObject;
        }
        return null;
    }

    public Vector3 GetNewFreerunningPoint() {
        
        
        return Vector3.zero;
    }

    public GameObject CanGrabLedgeLeap(Vector3 Dir) {
        Vector3 pos = Vector3.zero;

        if (Dir == Vector3.up)
            pos = new Vector3(owner.transform.position.x, owner.CurrentLedge.transform.position.y + .05f + owner.climbDistance, owner.transform.position.z) + owner.transform.forward * .6f;
        else if (Dir == Vector3.down)
            pos = new Vector3(owner.transform.position.x, owner.CurrentLedge.transform.position.y - .05f - owner.climbDistance, owner.transform.position.z) + owner.transform.forward * .6f;

        Ray ray = new(pos, Dir);

        Debug.DrawRay(pos, Dir);

        if (Physics.Raycast(ray, out var hit, owner.leapClimbDistance, owner.EdgeLayer)) {
            if (hit.collider.gameObject == owner.CurrentLedge) {
                Debug.LogError("Same Edge");
                return null;
            }

            owner.CurrentLedge = hit.collider.gameObject;
            return hit.collider.gameObject;
        }
        return null;
    }

    public Vector3 CanGoOntoLedge() {
        Vector3 pos = new Vector3(owner.transform.position.x, owner.CurrentLedge.transform.position.y + .1f, owner.transform.position.z);
        Ray ray = new(pos, owner.transform.forward);

        if (!Physics.Raycast(ray, out var hit, 3f)) {
            return owner.transform.position + owner.transform.forward * 1.2f + new Vector3(0, 2.4f, 0);
        }

        return Vector3.zero;
    }

    public bool LookAroundCorner() {
        var col = Physics.OverlapSphere(owner.transform.position, .2f, owner.EdgeLayer);

        foreach (var item in col) {
            if(item.gameObject != owner.CurrentLedge) {
                owner.CurrentLedge = item.gameObject;

                Debug.Log("Got a New Corner: " + item.gameObject.name);

                return true;
            }
        }

        return false;
    }
}