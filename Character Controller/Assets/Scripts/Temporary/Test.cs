using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject debugObject;
    public float angle;

    void Update() {
        Vector3 tmpdir = (debugObject.transform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(tmpdir, transform.up);

        Vector3 v = transform.position + Vector3.up * 4f - transform.position;
        Vector3 v2 = Quaternion.AngleAxis(angle * 3, Vector3.forward) * v;
        Vector3 v3 = Quaternion.AngleAxis(-angle * 3, Vector3.forward) * v;
        Vector3 f2 = Quaternion.AngleAxis(angle * 3, Vector3.up) * v;
        Vector3 f3 = Quaternion.AngleAxis(-angle * 3, Vector3.up) * v;
        Vector3 c2 = Quaternion.AngleAxis(angle * 3, Vector3.right) * v;
        Vector3 c3 = Quaternion.AngleAxis(-angle * 3, Vector3.right) * v;

        Debug.DrawRay(transform.position, v, Color.red);
        Debug.DrawRay(transform.position, v2, Color.blue);
        Debug.DrawRay(transform.position, v3, Color.blue);
        Debug.DrawRay(transform.position, f2, Color.blue);
        Debug.DrawRay(transform.position, f3, Color.blue);
        Debug.DrawRay(transform.position, c2, Color.blue);
        Debug.DrawRay(transform.position, c3, Color.blue);

        if (-dotProduct < Mathf.Cos(angle)) {
            Debug.Log("True");
        }
        else {
            Debug.Log("False");
        }
    }
}