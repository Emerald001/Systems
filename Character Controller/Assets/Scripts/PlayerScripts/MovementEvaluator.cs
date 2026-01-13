using System.Collections.Generic;
using UnityEngine;

public class MovementEvaluator {
    private readonly MovementManager owner;
    private readonly Transform ledgeCheck;

    public MovementEvaluator(MovementManager owner, Transform ledgeCheck) {
        this.owner = owner;
        this.ledgeCheck = ledgeCheck;
    }

    public Vector3 GetSlopeNormal() {
        Vector3 origin = owner.transform.position + new Vector3(0, .1f, 0);

        Physics.Raycast(origin, Vector3.down, out var hit, 1f);
        return IsGrounded() ? hit.normal : Vector3.up;
    }

    public bool IsGrounded() {
        return Physics.CheckSphere(owner.transform.position + new Vector3(0, .2f, 0), .5f);
    }

    public bool TouchedRoof() {
        return Physics.CheckSphere(owner.transform.position + new Vector3(0, 1.6f, 0), .5f);
    }

    public GameObject CollectableNearby() {
        List<Collider> tmp = new(Physics.OverlapSphere(ledgeCheck.transform.position, .5f));

        for (int i = tmp.Count - 1; i >= 0; i--) {
            if (tmp[i].GetComponent<IInteractable>() != null)
                continue;

            tmp.RemoveAt(i);
        }

        if (tmp.Count < 1)
            return null;

        return tmp[0].gameObject;
    }

    public GameObject GetLedge(Vector3 input, float dis, float rad) {
        if (input.magnitude < .1f)
            return null;

        if (Input.GetKey(KeyCode.Space)) {
            dis = 3;
            rad *= 3;
        }

        List<Collider> tmp = new(Physics.OverlapSphere(ledgeCheck.transform.position + (ledgeCheck.transform.TransformDirection(input.normalized * .7f) * dis), rad));
        if (tmp.Count < 1)
            return null;

        for (int i = tmp.Count - 1; i >= 0; i--) {
            if (tmp[i].gameObject.layer != 6) {
                tmp.RemoveAt(i);
                continue;
            }
        }

        if (tmp.Count < 1)
            return null;

        Collider closestPoint = tmp[0];
        foreach (var item in tmp) {
            if (item == closestPoint)
                continue;

            if (Vector3.Distance(ledgeCheck.transform.position + (ledgeCheck.transform.TransformDirection(input.normalized * .7f) * dis), 
                closestPoint.transform.position) > Vector3.Distance(ledgeCheck.transform.position + (ledgeCheck.transform.TransformDirection(input.normalized * .7f) * dis), item.transform.position))
                closestPoint = item;
        }

        return closestPoint.gameObject;
    }

    public Vector3 CanGoOntoLedge() {
        Vector3 pos = new(owner.transform.position.x, owner.CurrentLedge.transform.position.y + .1f, owner.transform.position.z);
        Ray ray = new(pos, owner.transform.forward);

        if (!Physics.Raycast(ray, out var hit, 3f))
            return owner.transform.position + owner.transform.forward * 1.2f + new Vector3(0, 2.4f, 0);

        return Vector3.zero;
    }
}