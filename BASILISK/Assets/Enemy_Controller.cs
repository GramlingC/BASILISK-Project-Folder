using UnityEngine;
using System.Collections;

public class Enemy_Controller : MonoBehaviour
{
    //private Rigidbody rb;

    private float enemySpeed; //How fast the enemy moves

    public float speedMultiplier = 1f; //To allow changing speed in unity editor

    public Vector3[] coords; //List of coordinates the enemy will travel to, in order.
    private int nextCoord;  //Index in coords of the next coordinate the enemy will pass through.
    private float yOffset;  //y coordinate the enemy starts at.  This is used to keep the enemy's y coordinate constant.

    // Use this for initialization
    void Start ()
    {
        yOffset = transform.position.y;

        //Sets nextCoord to element 1 of coords (0 is the starting position)
        if (coords.Length > 1)
            nextCoord = 1;
        enemySpeed = .05F;

        //May add some code to ensure that the enemy's orignal position is included in the set of coordiantes.
	}
	
	// Update is called once per frame
	void Update ()
    {
        // rb = GetComponent<Rigidbody>();
        //  Vector3 Enemy = new Vector3(-0.5f, 0, 0);
        //    rb.AddForce(Enemy);

        //Preliminary work for detecting player.  Need to figure out if the enemy can see from all directions or just the front.
        //RaycastHit hit;
        /*if(Physics.Raycast(transform.position, Vector3.forward, out hit, 1.5F))
        {
            //Debug.Log(hit.transform.gameObject.tag);
            if (hit.transform.gameObject.tag == "Player")
                RouteEnemy(hit.transform.position);
        }*/

        if (coords.Length > 1)
        {
            //Moves the enemy towards the next point.
            RouteEnemy(coords[nextCoord]);

            //If enemy has reached nextCoord, it updates the next coordinate index so the enemy changes direction.
            if (transform.position == coords[nextCoord])
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
            //If there are no coordinates, the enemy won't return to their original position.
            RouteEnemy(coords[0]);
        }
	}
    public void LightTrigger()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Light_Hitbox")
        {
            Debug.Log("Enemy hit by light");
            Destroy(gameObject);
        }
        //Having the Light_Hitbox check for enemies may be better for the framerate.
    }

    //Routes the enemy to point dest.
    private void RouteEnemy(Vector3 dest)
    {
        //Will likely be changed to allow routing around obstacles.
        //When sprites are added, code to change the faced direction may go here.
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(dest.x, yOffset, dest.z), enemySpeed * speedMultiplier);
    }
}
