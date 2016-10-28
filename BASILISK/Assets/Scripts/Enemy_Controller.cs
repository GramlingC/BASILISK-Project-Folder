using UnityEngine;
using System.Collections;

public abstract class Enemy_Controller : MonoBehaviour
{
    //private Rigidbody rb;

    public bool enemyChaseOff; //Temporary testing variable.  Setting this to true will prevent the enemy from chasing the player.

    private float enemySpeed; //How fast the enemy moves

    public float speedMultiplier = 1f; //To allow changing speed in unity editor

    public Vector3[] coords; //List of coordinates the enemy will travel to, in order.
    private int nextCoord;  //Index in coords of the next coordinate the enemy will pass through.
    private float yOffset;  //y coordinate the enemy starts at.  This is used to keep the enemy's y coordinate constant.

    private bool canSeePlayer;  //Becomes true when the enemy has spotted the player, and stays true until the player escapes
    //Used to trigger chasing/game-ending behavior.
    private GameObject player;  //Used to keep tabs on the players' position.

    // Use this for initialization
    void Start ()
    {
        yOffset = transform.position.y;

        //Sets nextCoord to element 1 of coords (0 is the starting position)
        if (coords.Length > 1)
            nextCoord = 1;
        enemySpeed = .05F;

        //May add some code to ensure that the enemy's orignal position is included in the set of coordiantes.

        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update ()
    {
        // rb = GetComponent<Rigidbody>();
        //  Vector3 Enemy = new Vector3(-0.5f, 0, 0);
        //    rb.AddForce(Enemy);

        if (canSeePlayer && !enemyChaseOff)
        {
            RouteEnemy(player.transform.position);
        }
        else
        {
            if (coords.Length > 1)
            {
                //Moves the enemy towards the next point.
                RouteEnemy(coords[nextCoord]);

                //If enemy has reached nextCoord, it updates the next coordinate index so the enemy changes direction.
                if (transform.position == new Vector3(coords[nextCoord].x, yOffset, coords[nextCoord].z))
                {
                    if (nextCoord < coords.Length - 1)
                        nextCoord++;
                    else
                        nextCoord = 0;
                }
            }
            else if (coords.Length == 1 && transform.position != coords[0])
            {
                //If the enemy's "patrol" is sitting in one place, they are returned to that place after chasing.
                //If there are no coordinates in coords, the enemy won't return to their original position.
                RouteEnemy(coords[0]);
            }
        }
	}

    //Different enemy types will have their own reactions to the light.
    public abstract void LightTrigger();

    //Routes the enemy to point dest.
    private void RouteEnemy(Vector3 dest)
    {
        //Will likely be changed to allow routing around obstacles.
        //When sprites are added, code to change the faced direction may go here.
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(dest.x, yOffset, dest.z), enemySpeed * speedMultiplier);
        
        
        //Currently is raycasting in the "forward" direction.  (The enemies will only spot the player if the player walks behind them)
        //The player will need some sort of collider for this to work.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 0.5F))
        {
            if (hit.transform.gameObject.tag == "Player")
                canSeePlayer = true;
            else
                canSeePlayer = false;
        }
        else
            canSeePlayer = false;

        //Other options for seeing player:
        /*
        //Uses player position as destination
        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.transform.position, out hit, 0.5F))
        {
            if (hit.transform.gameObject.tag == "Player")
                canSeePlayer = true;
            else
                canSeePlayer = false;
        }
        else
            canSeePlayer = false;
        */

        //The enemies will "give up" and return to their patrol once canSeePlayer becomes false.
        /*
        //Used code from player's light script to make a script that detects the player within the enemy's fov
        RaycastHit hit;
        //So that the enemy will follow if ray hits at least once, but won't if none hit
        bool sawPlayer = false;
        for (float degree = -30f; degree < 30f; degree += 6f)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1.5F))
            {
                Debug.DrawRay(transform.position, transform.forward, Color.green);
                Debug.Log(hit.transform.gameObject.tag);
                if (hit.transform.gameObject.tag == "Player")
                    sawPlayer = true;
            }
            if (sawPlayer == true)
                canSeePlayer = true;
            else
                canSeePlayer = false;
        }
        */

    }
}

