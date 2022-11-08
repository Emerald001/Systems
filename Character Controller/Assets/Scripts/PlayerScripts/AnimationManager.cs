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

    public AnimationManager(Animator animator) {
        this.animator = animator;
    }

    public void SetTrigger(string trigger) {
        animator.SetTrigger(trigger);
    }

    public void SetBool(string trigger, bool set) {
        animator.SetBool(trigger, set);
    }

    public void HandToLedge(GameObject Ledge) {


    }
}