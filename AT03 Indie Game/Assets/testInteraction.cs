using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testInteraction : MonoBehaviour, IInteractable
{
    #region bool defintion
    private bool exampleBool;
    public bool ExampleBool
    {
        get { return exampleBool; }
    }
    #endregion

    public delegate void InteractionDelegate();

    public event InteractionDelegate interactionEvent = delegate { };

    private void OnEnable()
    {
        interactionEvent = new InteractionDelegate(TestMethod);
        interactionEvent += TestMethodTwo;
    }

    private void OnDisable ()
    {
        interactionEvent -= TestMethod;
        interactionEvent -= TestMethodTwo;
    }



    private void Start()
    {
        interactionEvent = new InteractionDelegate(TestMethod);
        interactionEvent += TestMethodTwo;
    }

    public void Activate()
    {
        interactionEvent.Invoke();
    }

    private void TestMethod()
    {
        Debug.Log("delegate has been Executed");
    }

    private void TestMethodTwo()
    {
        Debug.Log("Second method has been Executed");
    }
}
