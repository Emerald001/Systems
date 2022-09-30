using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager
{
    public Animator animator;

    public AnimationManager(Animator animator) {
        this.animator = animator;
    }

    public void SetTrigger(string trigger) {
        animator.SetTrigger(trigger);
    }

    public void SetBool(string trigger, bool set) {
        animator.SetBool(trigger, set);
    }
}