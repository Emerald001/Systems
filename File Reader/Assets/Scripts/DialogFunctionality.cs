using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogFunctionality
{
    public void SetEvents() {
        EventManager<bool>.Subscribe(EventType.ON_TEST_EVENT, OnTestCallRun);
    }

    public void OnTestCallRun(bool yes) {
        Debug.Log("AHAHAAAA " + yes.ToString());
    }
}
