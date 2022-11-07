using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public float followSpeed;
    public Transform objectToFollow;

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, objectToFollow.transform.position, followSpeed * Time.deltaTime);
    }
}