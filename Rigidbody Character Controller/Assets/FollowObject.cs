using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform objectToFollow;

    void Update() {
        transform.position = objectToFollow.transform.position;
    }
}