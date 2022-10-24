using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour
{
    public LayerMask ground;
    public float minDis;
    public float legSpeed;
    public Transform YrotParent;

    public Transform leftLeg;
    public Transform rightLeg;

    public Transform leftFoot;
    public Transform rightFoot;

    private GameObject leftTarget;
    private GameObject rightTarget;

    private float leftDis;
    private float rightDis;

    private bool canInvoke = true;

    void Start() {
        leftTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftTarget.transform.localScale = new Vector3(.1f, .1f, .1f);

        rightTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightTarget.transform.localScale = new Vector3(.1f, .1f, .1f);
    }

    void Update() {
        if (Physics.Raycast(leftLeg.transform.position, Vector2.down, out var leftHit, 2f, ground)) {
            leftTarget.transform.position = leftHit.point;
            leftDis = Mathf.Abs(Vector3.Distance(leftFoot.transform.position, leftHit.point));
        }

        if (Physics.Raycast(rightLeg.transform.position, Vector2.down, out var rightHit, 2f, ground)) {
            rightTarget.transform.position = rightHit.point;
            rightDis = Mathf.Abs(Vector3.Distance(rightFoot.transform.position, rightHit.point));
        }

        leftFoot.transform.eulerAngles = new Vector3(leftFoot.transform.eulerAngles.x, YrotParent.eulerAngles.y, leftFoot.transform.eulerAngles.z);
        rightFoot.transform.eulerAngles = new Vector3(rightFoot.transform.eulerAngles.x, YrotParent.eulerAngles.y, rightFoot.transform.eulerAngles.z);

        if (leftDis > minDis && canInvoke) {
            canInvoke = false;
            StartCoroutine(moveLeftLeg());
        }
        if (rightDis > minDis && canInvoke) {
            canInvoke = false;
            StartCoroutine(moveRightLeg());
        }
    }

    public IEnumerator moveLeftLeg() {
        while (Vector3.Distance(leftFoot.position, leftTarget.transform.position + new Vector3(0, .1f, 0)) > .1f) {
            leftFoot.position = Vector3.MoveTowards(leftFoot.position, leftTarget.transform.position + new Vector3(0, .1f, 0), Time.deltaTime * legSpeed);
            yield return new WaitForEndOfFrame();
        }
        yield return null;

        canInvoke = true;
    }

    public IEnumerator moveRightLeg() {
        while (Vector3.Distance(rightFoot.position, rightTarget.transform.position + new Vector3(0, .1f, 0)) > .1f) {
            rightFoot.position = Vector3.MoveTowards(rightFoot.position, rightTarget.transform.position + new Vector3(0, .1f, 0), Time.deltaTime * legSpeed);
            yield return new WaitForEndOfFrame();
        }
        yield return null;

        canInvoke = true;
    }
}