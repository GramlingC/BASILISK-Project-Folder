using UnityEngine;
using System.Collections;

public class Bat_Controller : Enemy_Controller
{
    //Any methods and variables specific to bats go here.

    public override void LightTrigger()
    {
        //Debug.Log("Bat triggered.  Will fly away from player");
        isLightTriggered = true;
    }

    //Routes bats away from player when the light shines on them.
    public override void LightReaction()
    {
        Vector3 direction = Vector3.forward;

        //Attempt to restrict the bat's movement to four directions.
        //The bat should pick whether its x or z position is closer.
        //If x is closer, moves left or right, depending on player position.
        if (transform.position.x - player.transform.position.x < transform.position.z - player.transform.position.z)
            if (transform.position.x > player.transform.position.x)
                direction = Vector3.right;
            else
                direction = Vector3.left;
        else
            if (transform.position.z > player.transform.position.z)
                direction = Vector3.forward;
            else
                direction = Vector3.back;
        //Moves the bats in the calculated direction.
        RouteEnemy(transform.position + direction);

        //Enemies will still attempt to return to their route.  May not be so bad.
        //May need to do something to keep the enemy sprite from "flickering" between faced directions.
    }
}
