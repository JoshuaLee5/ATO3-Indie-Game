using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testInteraction2 : MonoBehaviour, IInteractable
{
    public testInteraction interaction;

    public void Start()
    {
        interaction.interactionEvent += TestMethodThree;
    }



    public void Activate()
    {

    }

    private void TestMethodThree()
    {
        Debug.Log("3rd method has been Executed");
    }

}
