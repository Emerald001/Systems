using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogFunctionality
{
    public void SetEvents() {
        EventManager.Subscribe(EventType.ON_TEST_EVENT, OnTestCallRun);
    }

    public void OnTestCallRun() {
        Debug.Log("AHAHAAAA");
    }
}
