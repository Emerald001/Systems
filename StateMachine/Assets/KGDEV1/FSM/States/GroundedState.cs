using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : EnemyState
{
    public GroundedState(StateMachine<Enemy> _owner) : base(_owner)
    {

    }

    public override void OnEnter()
    {
        Debug.Log("First State");
    }

    public override void OnUpdate()
    {
        SaySomthing();
        base.OnUpdate();
    }

    public void SaySomthing()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("In the Grounded State, Doing things");
        }
    }
}