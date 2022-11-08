using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager
{
    public Animator animator;
    public MovementManager owner;

    public Transform LeftHandTarget;
    public Transform RightHandTarget;
    public Transform LeftFootTarget;
    public Transform RightFootTarget;

    public float speed = 10;
    private GameObject CurrentLedge;

    public AnimationManager(MovementManager owner, Animator animator) {
        this.owner = owner;
        this.animator = animator;
    }

    public void OnUpdate() {
        if (!CurrentLedge)
            return;

        LeftHandTarget.position = Vector3.MoveTowards(LeftHandTarget.position, CurrentLedge.transform.position, speed * Time.deltaTime);
        RightHandTarget.position = Vector3.MoveTowards(RightHandTarget.position, CurrentLedge.transform.position, speed * Time.deltaTime);
    }

    public void HandToLedge(GameObject Ledge) {
        CurrentLedge = Ledge;
    }
}