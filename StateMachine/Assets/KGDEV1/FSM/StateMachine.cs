using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    private State<T> currentState;
    private Dictionary<System.Type, State<T>> stateDictionary = new Dictionary<System.Type, State<T>>();

    public T Controller { get; protected set; }

    public StateMachine(T _owner)
    {
        Controller = _owner;
    }

    public void SwitchState(System.Type _switcher)
    {
        if (currentState != null)
        {
            //currentState.OnExit();
        }

        if (stateDictionary.ContainsKey(_switcher))
        {
            var tmpState = stateDictionary[_switcher];
            tmpState.OnEnter();
            currentState = tmpState;
            return;
        }
    }

    public void AddState(System.Type _type, State<T> _state)
    {
        if (stateDictionary.ContainsValue(_state))
        {
            Debug.LogError("Already added State");
            return;
        }
        stateDictionary.Add(_type, _state);
    }

    public void RemoveState(System.Type _type)
    {
        if (!stateDictionary.ContainsKey(_type))
        {
            Debug.LogError("State not in the list");
            return;
        }
        stateDictionary.Remove(_type);
    }

    public void RunUpdate()
    {
        currentState.OnUpdate();
    }
}
