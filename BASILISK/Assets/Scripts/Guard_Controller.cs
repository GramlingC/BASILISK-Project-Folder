using UnityEngine;
using System.Collections;

public class Guard_Controller : Enemy_Controller
{
    //Any methods and variables specific to guards go here.

    public override void LightTrigger()
    {
        Debug.Log("Guard triggered.  Will apprehend/chase player.");

        canSeePlayer = true; //Guard can see player once shined with light
    }

    public override void LightReaction()
    {
        //Pretty much the same as if the player walked into their line of sight.  I think.
    }
}
