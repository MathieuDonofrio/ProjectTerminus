using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<T> : Singleton<T> where T:StateMachine<T>
{
    // Start is called before the first frame update
    protected State State;

    public void SetState(State state)
    {
        State = state;
        StartCoroutine(State.Start());
    }
   
}
