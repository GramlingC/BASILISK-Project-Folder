using UnityEngine;
using System.Collections;

public class Guard_Controller : Enemy_Controller
{
    //Any methods and variables specific to guards go here.

    public override void LightTrigger()
    {
        Debug.Log("Guard triggered.  Will apprehend/chase player.");
    }
}
