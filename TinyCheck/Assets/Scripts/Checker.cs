using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public Transform debugSphere;

    public float radius;
    public float waitTime;
    float dis;
    float rad;

    bool canInvoke = true;

    void Update() {
        Vector3 input = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        if (Input.GetKey(KeyCode.Space)) {
            dis = 3;
            rad = radius * 3;
        }
        else {
            dis = 1;
            rad = radius;
        }

        if (input.magnitude > 0) {
            var pos = transform.position + (input.normalized * .7f) * dis;

            debugSphere.gameObject.SetActive(true);
            debugSphere.position = pos;
            debugSphere.localScale = new Vector3(rad * 2, rad * 2, rad * 2);

            if (!canInvoke)
                return;

            SphereCast(input, dis, rad);
        }
        else {
            debugSphere.gameObject.SetActive(false);
        }
    }

    void SphereCast(Vector3 input, float dis, float rad) {
        var pos = transform.position + (input.normalized * .7f) * dis;

        var tmp = Physics.OverlapSphere(pos, rad);

        if (tmp.Length < 1)
            return;

        var closestPoint = tmp[0];
        foreach (var item in tmp) {
            if (item == closestPoint)
                continue;

            if (Vector3.Distance(pos, closestPoint.transform.position) > Vector3.Distance(pos, item.transform.position)) {
                closestPoint = item;
            }
        }

        transform.position = closestPoint.transform.position;

        canInvoke = false;
        if (dis > 1)
            StartCoroutine(Timer(waitTime * 2));
        else
            StartCoroutine(Timer(waitTime));
    }

    IEnumerator Timer(float seconds) {
        yield return new WaitForSeconds(seconds);

        canInvoke = true;
    }
}