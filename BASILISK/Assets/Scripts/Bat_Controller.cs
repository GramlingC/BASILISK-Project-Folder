using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bat_Controller : MonoBehaviour
{

    public bool enemyChaseOff; //Temporary testing variable.  Setting this to true will prevent the enemy from chasing the player.

    private float enemySpeed; //How fast the enemy moves

    public float speedMultiplier = 1f; //To allow changing speed in unity editor
    public float runMultiplier; //Could allow for different speeds (ie. running and chasing are faster than patrol)

    public Animator enemy_sprite;

    public Vector3[] coords; //Array of coordinates the enemy will travel to, in order.
    private float yOffset;  //y coordinate the enemy starts at.  This is used to keep the enemy's y coordinate constant.

    public float chaseTimerMax = 3f; //Seconds till give up chase
    private float chaseTimer;
    private bool isChasing;
    private bool isEscaping;

    private List<Vector3> patrolPath; //List of all coordinates the enemy will pass through during patrol - includes coordinates
    //between those in coords
    private int nextPatrolCoord;  //Index in coords of the next coordinate the enemy will pass through.

    private List<Vector3> returnPath; //Temporarily stores a return path when the enemy is returning.
    private int nextReturnCoord;  //Index of the next coordinate in the return path.

    private Vector3 nextChasePos; //Temporarily stores the next index the guard will go when chasing the player.

    private Vector3 direction; //Direction the player object faces/ currently not working

    private int enemyState; //0 for patrol, 1 for chase, 2 for return
    private bool isLightTriggered;  //True if enemy is reacting to light.  Affects enemyState.
    public bool canSeePlayer;  //Becomes true when the enemy has spotted the player.  Affects enemy state.
    private bool atRoundCoord; //True if enemy is at a position that exactly corresponds to a grid node.
    private GameObject player;  //Used to keep tabs on the players' position.

    private A_Pathfinding pathfinder;

    //private int nextState; //The next state the enemy will move to - used since guard won't change behavior until reaching a round
    //coordinate.

    // Use this for initialization
    void Start()
    {
        patrolPath = new List<Vector3>();
        yOffset = transform.position.y;
        atRoundCoord = true;
        pathfinder = GameObject.Find("PathfindingObj").GetComponent<A_Pathfinding>();
        nextChasePos = transform.position;

        direction = transform.forward;

        //Sets up the patrol path.  Keeps enemy movement unitized.
        if (coords.Length == 0)
            Debug.Log("Coordinates are empty.  Bat has no patrol.");  //Code may or may not support patrolless enemies in the future.
        else if (coords.Length == 1)
        {
            patrolPath.Add(coords[0]);
            nextPatrolCoord = 0;  //-1???
        }
        else
        {
            for (int i = 0; i < coords.Length; i++)
            {
                Vector3 tempNext;
                if (i < coords.Length - 1)
                {
                    tempNext = coords[i + 1];
                }
                else
                {
                    tempNext = coords[0];
                }

                //Debugging check for diagonal (non-four-direction) patrol path
                if (tempNext.x != coords[i].x && tempNext.z != coords[i].z)
                {
                    Debug.Log("Diagonal path: " + coords[i] + " " + tempNext);
                    break;
                }
                //Debugging check
                if ((tempNext.x - coords[i].x) % 1 != 0 || (tempNext.z - coords[i].z) % 1 != 0)
                {
                    Debug.Log("Non-grid distance: " + coords[i] + " " + tempNext);
                    break;
                }
                //Need debugging to check for coordinates not placed on grid nodes.

                if (coords[i].x != tempNext.x)
                {
                    for (int j = 0; j < Mathf.Abs(tempNext.x - coords[i].x); j++)
                    {
                        if (coords[i].x < tempNext.x)
                            patrolPath.Add(new Vector3(coords[i].x + j, yOffset, coords[i].z));
                        else
                            patrolPath.Add(new Vector3(coords[i].x - j, yOffset, coords[i].z));
                    }
                }
                else if (coords[i].z != tempNext.z)
                {
                    for (int j = 0; j < Mathf.Abs(tempNext.z - coords[i].z); j++)
                    {
                        if (coords[i].z < tempNext.z)
                            patrolPath.Add(new Vector3(coords[i].x, yOffset, coords[i].z + j));
                        else
                            patrolPath.Add(new Vector3(coords[i].x, yOffset, coords[i].z - j));
                    }
                }
                else
                    Debug.Log("There are two of the same coordinate in a row.");
                nextPatrolCoord = 0;
            }//End of loop adding coordinates
        }
        //nextPatrolCoord = 1;
        enemySpeed = 1F;

        //initializes the return path for use later.
        returnPath = new List<Vector3>();

        //May add some code to ensure that the enemy's orignal position is included in the set of coordiantes.

        player = GameObject.FindGameObjectWithTag("Player");

        isLightTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("enemy state: " + enemyState);

        //Something may be wrong with atRoundCoord
        //Or maybe chase.  The game always seems to crash right after the "chase" if statement is comepleted

        //Debug.Log(enemyState);
        if (enemyState == 0)
        {
            //Debug.Log("Starting patrol movement");
            //Moves enemy to next coordinate in the patrolPath list
            RouteEnemy(patrolPath[nextPatrolCoord]);
            //If enemy has reached nextPatrolCoord, it updates the next coordinate index so the enemy changes direction.
            if (transform.position == new Vector3(patrolPath[nextPatrolCoord].x, yOffset, patrolPath[nextPatrolCoord].z))
            {
                //May need to update to allow for "standing still" patrol.
                atRoundCoord = true;
                if (nextPatrolCoord < patrolPath.Count - 1)
                    nextPatrolCoord++;
                else
                    nextPatrolCoord = 0;
            }
            else
                atRoundCoord = false;

            //If the guard sees player or is hit by the light, they will chase once they reach a "round" coordinate.
            if (atRoundCoord)
            {
                if (isLightTriggered)
                {
                    //Debug.Log("Going to chasing...");
                    enemyState = 1;
                    chaseTimer = chaseTimerMax;
                    isEscaping = true;
                    canSeePlayer = false;
                    isLightTriggered = false;
                    setEscapePos();
                    //Debug.Log("Successfully gone to chasing.");
                }
                else if (canSeePlayer)
                {
                    enemyState = 1;
                    chaseTimer = chaseTimerMax;
                    isChasing = true;
                    canSeePlayer = false;
                    isLightTriggered = false;
                    setChasePos();
                }
            }
            //Debug.Log("Ending Patrol movement");
        }
        //Chase
        else if (enemyState == 1)
        {
            //Debug.Log("Starting chase movement");
            atRoundCoord = false;
            //atRoundCoord = true; //TEMP!!!
            //Move enemy in direction closest to player position.
            //Debug.Log("Entering chase");
            //Debug.Log(nextChasePos);

            if (transform.position == nextChasePos)
                atRoundCoord = true;
            else
                RouteEnemy(nextChasePos);

            if (chaseTimer <= 0)
            {
                isChasing = false;
                isEscaping = false;
            }

            //Once they reach a round coordinate, check for light and player.  If neither, guard will return using pathfinding.
            if (atRoundCoord) {
                if (isLightTriggered)
                {
                    //Debug.Log("Round " + enemyState + " " + transform.position);
                    //Debug.Log(transform.position);
                    chaseTimer = chaseTimerMax;
                    canSeePlayer = false;
                    isLightTriggered = false;
                    setEscapePos();
                }
                else if (canSeePlayer)
                {
                    //Debug.Log("Round " + enemyState + " " + transform.position);
                    //Debug.Log(transform.position);
                    chaseTimer = chaseTimerMax;
                    canSeePlayer = false;
                    isLightTriggered = false;
                    setChasePos();
                }
                else if (isEscaping)
                {
                    chaseTimer -= 1;
                    setEscapePos();
                }
                else if (isChasing)
                {
                    chaseTimer -= 1;
                    setChasePos();
                }
            }
            else if (atRoundCoord && patrolPath.Contains(transform.position))  //Can save from running the pathfinding algorithm.
            {
                enemyState = 0;
                nextPatrolCoord = patrolPath.IndexOf(transform.position);
            }
            else if (atRoundCoord)
            {
                //Debug.Log("Next patrol coord: " + patrolPath[nextPatrolCoord]);
                enemyState = 2;
                returnPath = pathfinder.FindPath(transform.position, patrolPath[nextPatrolCoord]);
                //Debug.Log("Last coord in Return Path: " + returnPath[returnPath.Count - 1]);
                foreach (Vector3 vec in returnPath)
                {
                    Debug.Log("Return path: " + vec);
                }
                nextReturnCoord = 0;
            }
            //Debug.Log("Ending chase movement");
            //It doesn't always happen, but the game seems to crash after the "Ending chase movement" statement is executed.
        }
        //Return
        else if (enemyState == 2)
        {
            //Debug.Log("Enemy position: " + transform.position);
            //Debug.Log("Starting return movement");
            //Debug.Log("Return path length: " + returnPath.Count);
            //Moves enemy to next coordinate in returnPath list.
            RouteEnemy(returnPath[nextReturnCoord]);
            //If enemy has reached nextReturnCoord, it updates the next coordinate index so the enemy changes direction.
            if (transform.position == new Vector3(returnPath[nextReturnCoord].x, yOffset, returnPath[nextReturnCoord].z))
            {
                //May need to update to allow for "standing still" patrol.
                atRoundCoord = true;
                if (nextReturnCoord < returnPath.Count - 1)
                    nextReturnCoord++;
                //else
                //    Debug.Log("Reached end of return path");
            }
            else
                atRoundCoord = false;
            //Once the enemy reaches a round coordinate, start chasing if triggered by sighting player or being hit by lifght
            if (atRoundCoord && isLightTriggered)
            {
                //Debug.Log("Going to chasing...");
                enemyState = 1;
                canSeePlayer = false;
                isLightTriggered = false;
                setEscapePos();
                //Debug.Log("Successfully gone to chase.");
            }
            else if (atRoundCoord && canSeePlayer)
            {
                enemyState = 1;
                canSeePlayer = false;
                isLightTriggered = false;
                setChasePos();
            }
            else if (atRoundCoord && patrolPath.Contains(new Vector3(transform.position.x, yOffset, transform.position.z)))
            {
                //Debug.Log("Going to patrol...");
                enemyState = 0;
                nextPatrolCoord = patrolPath.IndexOf(new Vector3(transform.position.x, yOffset, transform.position.z));
                //Debug.Log("Successfully gone to patrol.");
            }
            //Debug.Log("Ending return movement");
        }
    }

    //Function that needs to be called by the player when a raycast hits this enemy.  Might use a "Guard" tag or something.
    public void LightTrigger()
    {
        isLightTriggered = true;
    }

    //Routes the enemy to point dest.
    private void RouteEnemy(Vector3 dest)
    {
        //Will likely be changed to allow routing around obstacles.
        //When sprites are added, code to change the faced direction may go here.

        direction = new Vector3(dest.x, yOffset, dest.z);

        if (direction != transform.position)
        {
            enemy_sprite.SetBool("isWalking", true);

            //Define horizontal and vertical distances
            float dist_H = transform.position.x - dest.x;
            float dist_V = transform.position.z - dest.z;

            float speed;
            if (isChasing || isEscaping)
                speed = runMultiplier;
            else
                speed = speedMultiplier;


            //Move horizontally or vertically towards player
            if (Mathf.Abs(dist_H) > Mathf.Abs(dist_V))
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(dest.x, yOffset, transform.position.z), enemySpeed * speed * Time.deltaTime);
                if (dist_H > 0)
                    enemy_sprite.SetInteger("Direction", 4);
                else
                    enemy_sprite.SetInteger("Direction", 2);
            }
            else if (dist_V == dist_H)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, yOffset, dest.z), enemySpeed * speed * Time.deltaTime);
                if (dist_H > 0)
                    enemy_sprite.SetInteger("Direction", 4);
                else
                    enemy_sprite.SetInteger("Direction", 2);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, yOffset, dest.z), enemySpeed * speed * Time.deltaTime);
                if (dist_V > 0)
                    enemy_sprite.SetInteger("Direction", 1);
                else
                    enemy_sprite.SetInteger("Direction", 3);
            }
        }
        else
            enemy_sprite.SetBool("isWalking", false);

        //transform.position = Vector3.MoveTowards(transform.position, direction, enemySpeed * speedMultiplier);


        //Code to make enemy face movement
        Vector3 enemy_ray = direction - transform.position;
        enemy_ray.y = 0.0f;
        if (enemy_ray != new Vector3(0, 0, 0))
        {
            Quaternion newRotation = Quaternion.LookRotation(enemy_ray);
            GetComponent<Rigidbody>().MoveRotation(newRotation);
        }

        RaycastHit hit;
        //So that the enemy will follow if ray hits at least once, but won't if none hit
        float distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
        //bool sawPlayer = false;

        if (distanceFromPlayer < 3f)
        {
            Vector3 lookDirection = transform.forward;
            float angleBetween = Mathf.Atan2(player.transform.position.z - transform.position.z, player.transform.position.x - transform.position.x);
            float angleLooking = Mathf.Atan2(lookDirection.z, lookDirection.x);
            //print("Angle between: " + ((angleBetween - angleLooking) * 180 / Mathf.PI));
        }
        for (float degree = -45f; degree < 45f; degree += 5f)
        {
            Vector3 LookDirection = Quaternion.AngleAxis(degree, Vector3.up) * transform.forward;
            if (Physics.Raycast(transform.position, LookDirection, out hit, 3F))
            {
                //Debug.Log(hit.transform.gameObject.tag);
                if (hit.transform.gameObject.tag == "Player")
                    canSeePlayer = true;
            }
        }
        /*if (sawPlayer == true)
            canSeePlayer = true;
        else
            canSeePlayer = false;*/
    }

    private void setEscapePos()
    {
        //Vector3 escapeDirection = transform.position + (transform.position - player.transform.position);
        //Creates dictionary of preferences regarding which direction to go
        Dictionary<Vector3, int> preferences = new Dictionary<Vector3, int>();
        preferences.Add(Vector3.right, 2);
        preferences.Add(Vector3.left, 2);
        preferences.Add(Vector3.forward, 2);
        preferences.Add(Vector3.back, 2);

        //Higher preference for closer horizontal direction
        if (player.transform.position.x - transform.position.x < 0)
        {
            if (Mathf.Abs(player.transform.position.z - transform.position.z) > Mathf.Abs(player.transform.position.x - transform.position.x))
            {
                //Even higher if farther away horizontally than vertically
                preferences[Vector3.right] += 2;
                preferences[Vector3.left]--;
            }
            else
                preferences[Vector3.right]++;
        }
        else
        {
            if (Mathf.Abs(player.transform.position.z - transform.position.z) > Mathf.Abs(player.transform.position.x - transform.position.x))
            {
                preferences[Vector3.left] += 2;
                preferences[Vector3.right]--;
            }
            else
                preferences[Vector3.left]++;
        }
        //Same but vertically
        if (player.transform.position.z - transform.position.z < 0)
        {
            if (Mathf.Abs(player.transform.position.z - transform.position.z) < Mathf.Abs(player.transform.position.x - transform.position.x))
            {
                preferences[Vector3.forward] += 2;
                preferences[Vector3.back]--;
            }
            else
                preferences[Vector3.forward]++;
        }
        else
        {
            if (Mathf.Abs(player.transform.position.z - transform.position.z) < Mathf.Abs(player.transform.position.x - transform.position.x))
            {
                preferences[Vector3.back] += 2;
                preferences[Vector3.forward]--;
            }
            else
                preferences[Vector3.back]++;
        }
        //To find the vector with the highest preference
        KeyValuePair<Vector3, int> l = new KeyValuePair<Vector3, int>(Vector3.zero, 0);
        foreach (KeyValuePair<Vector3, int> entry in preferences)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, entry.Key, out hit, 1.5F))
            {
                if (hit.transform.gameObject.tag == "Player")
                    if (entry.Value > l.Value)
                        l = entry;
            }
            else if (entry.Value > l.Value)
                l = entry;
        }
        nextChasePos = transform.position + l.Key;
    }
    private void setChasePos()
    {
        //Creates dictionary of preferences regarding which direction to go
        Dictionary<Vector3, int> preferences = new Dictionary<Vector3, int>();
        preferences.Add(Vector3.right, 2);
        preferences.Add(Vector3.left, 2);
        preferences.Add(Vector3.forward, 2);
        preferences.Add(Vector3.back, 2);

        //Higher preference for closer horizontal direction
        if (player.transform.position.x - transform.position.x > 0)
        {
            if (Mathf.Abs(player.transform.position.z - transform.position.z) < Mathf.Abs(player.transform.position.x - transform.position.x))
            {
                //Even higher if farther away horizontally than vertically
                preferences[Vector3.right] += 2;
                preferences[Vector3.left]--;
            }
            else
                preferences[Vector3.right]++;
        }
        else
        {
            if (Mathf.Abs(player.transform.position.z - transform.position.z) < Mathf.Abs(player.transform.position.x - transform.position.x))
            {
                preferences[Vector3.left] += 2;
                preferences[Vector3.right]--;
            }
            else
                preferences[Vector3.left]++;
        }
        //Same but vertically
        if (player.transform.position.z - transform.position.z > 0)
        {
            if (Mathf.Abs(player.transform.position.z - transform.position.z) > Mathf.Abs(player.transform.position.x - transform.position.x))
            {
                preferences[Vector3.forward] += 2;
                preferences[Vector3.back]--;
            }
            else
                preferences[Vector3.forward]++;
        }
        else
        {
            if (Mathf.Abs(player.transform.position.z - transform.position.z) > Mathf.Abs(player.transform.position.x - transform.position.x))
            {
                preferences[Vector3.back] += 2;
                preferences[Vector3.forward]--;
            }
            else
                preferences[Vector3.back]++;
        }
        //To find the vector with the highest preference
        KeyValuePair<Vector3, int> l = new KeyValuePair<Vector3, int>(Vector3.zero, 0);
        foreach (KeyValuePair<Vector3, int> entry in preferences)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, entry.Key, out hit, 1.5F))
            {
                if (hit.transform.gameObject.tag == "Player")
                    if (entry.Value > l.Value)
                        l = entry;
            }
            else if (entry.Value > l.Value)
                l = entry;
        }
        nextChasePos = transform.position + l.Key;
    }
}
