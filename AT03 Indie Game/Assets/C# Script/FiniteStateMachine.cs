using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
    protected IState entryState;
    public IState CurrentState { get; private set; }

    protected virtual void Awake()
    {
        
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (entryState != null)
        {
            SetState(entryState);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.OnStateUpdate();
        }
    }

    public void SetState(IState state)
    { 
        if(CurrentState != null)
        {
            CurrentState.OnStateExit();
        }
        CurrentState = state;
        CurrentState.OnStateEnter();
    }

    protected virtual void OnDrawGizmos()
    {
        if (CurrentState != null)
        {
            CurrentState.DrawStateGizmo();
        }
    }

}


public interface IState
{
    public void OnStateEnter();
    public void OnStateUpdate();
    public void OnStateExit();
    public void DrawStateGizmo();
    
}

