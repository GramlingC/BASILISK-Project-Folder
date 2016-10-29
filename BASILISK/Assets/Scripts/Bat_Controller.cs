using UnityEngine;
using System.Collections;

public class Bat_Controller : Enemy_Controller
{
    //Any methods and variables specific to bats go here.

    public override void LightTrigger()
    {
        /*
        Debug.Log("Bat triggered.  Will fly away from player");
        if (!isLightTriggered)
        {
            isLightTriggered = true;
            StartCoroutine(LightTriggerTime());
        }
        //Check enemy position relative to player and next coordinate.  If player is in between them, the enemy will reverse.
        //Update nextCoord with that info.
        */
    }

    /*IEnumerator LightTriggerTime()
    {
        yield return new WaitForSeconds(3F);
        isLightTriggered = false;
    }*/

    public override void LightReaction()
    {
        /*Debug.Log("Bat triggered.  Will fly away from player");
        RouteEnemy(coords[nextCoord]);*/
    }
}
