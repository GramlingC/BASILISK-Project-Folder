﻿using UnityEngine;
using System.Collections;

public abstract class Enemy_Controller : MonoBehaviour
{
    //private Rigidbody rb;

    public bool enemyChaseOff; //Temporary testing variable.  Setting this to true will prevent the enemy from chasing the player.

    private float enemySpeed; //How fast the enemy moves

    public float speedMultiplier = 1f; //To allow changing speed in unity editor

	public Animator enemy_sprite;

    public Vector3[] coords; //List of coordinates the enemy will travel to, in order.
    protected int nextCoord;  //Index in coords of the next coordinate the enemy will pass through.
    protected float yOffset;  //y coordinate the enemy starts at.  This is used to keep the enemy's y coordinate constant.

    private Vector3 direction; //Direction the player object faces/ currently not working

    protected bool isLightTriggered;  //True if enemy is reacting to light.
    public bool canSeePlayer;  //Becomes true when the enemy has spotted the player, and stays true until the player escapes / public for Guard_Controller access
    //Used to trigger chasing/game-ending behavior.
    protected GameObject player;  //Used to keep tabs on the players' position.

    // Use this for initialization
    void Start ()
    {
        yOffset = transform.position.y;

        direction = transform.forward;

        //Sets nextCoord to element 1 of coords (0 is the starting position)
        if (coords.Length > 1)
            nextCoord = 1;
        enemySpeed = 1F;

        //May add some code to ensure that the enemy's orignal position is included in the set of coordiantes.

        player = GameObject.FindGameObjectWithTag("Player");

        isLightTriggered = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Bats will run away from light even if they can see player.
        //There may be a better way of doing this...  Guards may react to light no differently than if they saw the player.
        //Subclassing may not be the way to go...
        if(isLightTriggered)
        {
            LightReaction();
            isLightTriggered = false;
        }
		else if (canSeePlayer && !enemyChaseOff)
        {
            RouteEnemy(player.transform.position);
        }
        else
        {
			if (coords.Length > 1) {
				//Moves the enemy towards the next point.
				RouteEnemy (coords [nextCoord]);

				//If enemy has reached nextCoord, it updates the next coordinate index so the enemy changes direction.
				if (transform.position == new Vector3 (coords [nextCoord].x, yOffset, coords [nextCoord].z)) {
					if (nextCoord < coords.Length - 1)
						nextCoord++;
					else
						nextCoord = 0;
				}
			} else if (coords.Length == 1 && transform.position != coords [0]) {
				//If the enemy's "patrol" is sitting in one place, they are returned to that place after chasing.
				//If there are no coordinates in coords, the enemy won't return to their original position.
				RouteEnemy (coords [0]);
			} else
				RouteEnemy (transform.position);
        }
	}

    //Different enemy types will have their own reactions to the light.
    public abstract void LightTrigger();

    public abstract void LightReaction();

    //Routes the enemy to point dest.
    protected void RouteEnemy(Vector3 dest)
	{
		//Will likely be changed to allow routing around obstacles.
		//When sprites are added, code to change the faced direction may go here.
        
		direction = new Vector3 (dest.x, yOffset, dest.z);

		if (direction != transform.position) {
			enemy_sprite.SetBool ("isWalking", true);

			//Define horizontal and vertical distances
			float dist_H = transform.position.x - dest.x;
			float dist_V = transform.position.z - dest.z;


			//Move horizontally or vertically towards player
			if (Mathf.Abs (dist_H) > Mathf.Abs (dist_V)) {
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (dest.x, yOffset, transform.position.z), enemySpeed * speedMultiplier * Time.deltaTime);
				if (dist_H > 0)
					enemy_sprite.SetInteger ("Direction", 4);
				else
					enemy_sprite.SetInteger ("Direction", 2);
			} else if (dist_V == dist_H) {
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, yOffset, dest.z), enemySpeed * speedMultiplier * Time.deltaTime);
				if (dist_H > 0)
					enemy_sprite.SetInteger ("Direction", 4);
				else
					enemy_sprite.SetInteger ("Direction", 2);
			} else {
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, yOffset, dest.z), enemySpeed * speedMultiplier * Time.deltaTime);
				if (dist_V > 0)
					enemy_sprite.SetInteger ("Direction", 1);
				else
					enemy_sprite.SetInteger ("Direction", 3);
			}
		}
		else
			enemy_sprite.SetBool ("isWalking", false);

		//transform.position = Vector3.MoveTowards(transform.position, direction, enemySpeed * speedMultiplier);
        

		//Code to make enemy face movement
		Vector3 enemy_ray = direction - transform.position;
		enemy_ray.y = 0.0f;
		if (enemy_ray != new Vector3 (0, 0, 0)) {
			Quaternion newRotation = Quaternion.LookRotation (enemy_ray);
			GetComponent<Rigidbody> ().MoveRotation (newRotation);
		}

		RaycastHit hit;
        //So that the enemy will follow if ray hits at least once, but won't if none hit
        float distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
		bool sawPlayer = false;

        if (distanceFromPlayer < 3f) {
            Vector3 lookDirection = transform.forward;
            float angleBetween = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x);
            float angleLooking = Mathf.Atan2(lookDirection.y, lookDirection.x);
// print("Angle between: " + ((angleBetween - angleLooking) * 180 / Mathf.PI));
        }
		for (float degree = -45f; degree < 45f; degree += 5f) 
		{
			Vector3 LookDirection = Quaternion.AngleAxis (degree, Vector3.up) * transform.forward;
			if (Physics.Raycast (transform.position, LookDirection, out hit, 3F)) {
				//Debug.Log (hit.transform.gameObject.tag);
				if (hit.transform.gameObject.tag == "Player")
					sawPlayer = true;
			}
		}
		if (sawPlayer == true)
			canSeePlayer = true;
		else
			canSeePlayer = false;
    }
}

