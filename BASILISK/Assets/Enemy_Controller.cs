using UnityEngine;
using System.Collections;

public class Enemy_Controller : MonoBehaviour
{
    //private Rigidbody rb;

    private float enemySpeed; //How fast the enemy moves

    public float speedMultiplier = 1f; //To allow changing speed in unity editor

    public Vector3[] coords; //List of coordinates the enemy will travel to, in order.
    private int nextCoord;  //Index in coords of the next coordinate the enemy will pass through.

    // Use this for initialization
    void Start ()
    {
	    //Sets nextCoord to element 1 of coords (0 is the starting position)
        if (coords.Length > 1)
            nextCoord = 1;
        enemySpeed = .05F;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // rb = GetComponent<Rigidbody>();
        //  Vector3 Enemy = new Vector3(-0.5f, 0, 0);
        //    rb.AddForce(Enemy);

        //In case the enemy just stands in one place.
        if (coords.Length > 1)
        {
            //Moves the enemy towards the next point.
            transform.position = Vector3.MoveTowards(transform.position, coords[nextCoord], enemySpeed*speedMultiplier);
            //Possibly temporary, as this does not deal with obstacles.

            //If enemy has reached nextCoord, it updates the next coordinate index so the enemy changes direction.
            if (transform.position == coords[nextCoord])
            {
                if (nextCoord < coords.Length - 1)
                    nextCoord++;
                else
                    nextCoord = 0;
            }
        }
	}
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Light_Hitbox")
        {
            Destroy(gameObject);
        }
        //Having the Light_Hitbox check for enemies may be better for the framerate.
    }
}
