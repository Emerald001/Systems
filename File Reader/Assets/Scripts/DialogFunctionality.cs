using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogFunctionality
{
    public FileReader Owner;

    public void SetEvents() {
        EventManager<bool>.Subscribe(EventType.ON_TEST_EVENT, OnTestCallRun);
        EventManager<float>.Subscribe(EventType.SET_TYPE_TIME, SetSpeed);
    }

    public void OnTestCallRun(bool yes) {

    }

    public void SetSpeed(float speed) {
        Owner.currentTimeBetweenChars = speed;
    }
}