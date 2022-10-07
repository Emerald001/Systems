using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAnimation : MonoBehaviour
{
    public LayerMask ground;
    public float minDis;

    public Transform leftLeg;
    public Transform rightLeg;

    public Transform leftFoot;
    public Transform RightFoot;

    private GameObject leftTarget;
    private GameObject rightTarget;

    private float leftDis;
    private float rightDis;

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
            rightDis = Mathf.Abs(Vector3.Distance(RightFoot.transform.position, rightHit.point));
        }

        if (leftDis > minDis) {
            StartCoroutine(moveLeftLeg());
        }
        if (rightDis > minDis) {
            StartCoroutine(moveRightLeg());
        }
    }

    public IEnumerator moveLeftLeg() {
        while (leftFoot.position != leftTarget.transform.position + new Vector3(0, .1f, 0)) {
            leftFoot.position = Vector3.MoveTowards(leftFoot.position, leftTarget.transform.position + new Vector3(0, .1f, 0), Time.deltaTime * 5);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public IEnumerator moveRightLeg() {
        while (RightFoot.position != rightTarget.transform.position + new Vector3(0, .1f, 0)) {
            RightFoot.position = Vector3.MoveTowards(RightFoot.position, rightTarget.transform.position + new Vector3(0, .1f, 0), Time.deltaTime * 5);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}