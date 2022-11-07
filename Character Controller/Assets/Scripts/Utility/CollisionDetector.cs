using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    [HideInInspector] public List<GameObject> CollisionsThisFrame = new();

    private void FixedUpdate() {
        CollisionsThisFrame.Clear();
    }

    private void OnTriggerStay(Collider c) {
        CollisionsThisFrame.Add(c.gameObject);
    }
}