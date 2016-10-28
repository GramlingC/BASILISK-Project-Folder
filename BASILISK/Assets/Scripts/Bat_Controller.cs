using UnityEngine;
using System.Collections;

public class Bat_Controller : Enemy_Controller
{
    //Any methods and variables specific to bats go here.

    public override void LightTrigger()
    {
        Debug.Log("Bat triggered.  Will fly away from player");
    }
}
