using UnityEngine;
using System.Collections;

public class Player_Controller : MonoBehaviour {
    
	public float speedMultiplier = 5f;
    public int numberOfRays = 10;

	int floorMask;
    Rigidbody player_rb;
    Light player_light;
    public Animator player_sprite;
    public float lightDiminishPerSecond = 5f;
    public float lightCrankedPerSecond = 5f;
    public float secondsUntilStartDiminish = 3f;
    public float maxLightAngle = 60;
    private float currentSecondsBeforeDiminish;

    private float crankTime;
    private float secondsCrankUntilDiminish;
    public string crankKeyPress = "c";
    public bool crankingLight;
    public bool lightIsOn;
    public string lightSwitching = "z";

	public GameObject loss_screen;
	private bool loss;
    //Used to access methods from A_Pathfinding to constrain movement
    private A_Pathfinding pathfinder;
    private float leftBound;
    private float rightBound;
    private float bottomBound;
    private float topBound;

	//Used for Gameover condition

    // Use this for initialization
    void Start () {
        player_rb = GetComponent<Rigidbody>();
        lightIsOn = true;
        crankingLight = false;
        currentSecondsBeforeDiminish = secondsUntilStartDiminish;
        crankTime = 0;
        secondsCrankUntilDiminish = 1;
		loss = false;

        //Sets boundaries
        pathfinder = GameObject.Find("PathfindingObj").GetComponent<A_Pathfinding>();
        leftBound = pathfinder.GetLeftBound();
        rightBound = pathfinder.GetRightBound();
        bottomBound = pathfinder.GetBottomBound();
        topBound = pathfinder.GetTopBound();
	}
    
    void Awake () {
        floorMask = LayerMask.GetMask("Floor");
        player_light = GetComponentInChildren<Light>();
	}
    void FixedUpdate()
    {
        Turning();
    }
    void Update() {

        if (lightIsOn) { 
        //Call CastLight
            CastLight();
            crankLight();
            gradualLightDiminish();
        }
        //Player Movement
        playerActivateDeactivateLight();
        if (!crankingLight) 
            playerMovement();
        else
            player_sprite.SetBool("isWalking", false);
    }
	void OnCollisionEnter (Collision col)
	{
		print("Collided");
		if (col.gameObject.tag == "Guard" || col.gameObject.tag == "Bat") {
		loss_screen.transform.Translate(0f,0f,.5f);
		loss = true;
		Time.timeScale = 0;
		//print("Collided");

		}

	}
	void OnGUI(){
		if (loss == true) 
		{
			GUI.Label (new Rect (200, 200, 300, 400), "Game Over press R to restart");
		}
		
	}
    //Functions dealing wih diminishing light. Public functions are so that other scripts can call it also
    public void diminishLight(float angleAmount) {
        player_light.spotAngle -= angleAmount;
    }

    private void gradualLightDiminish() {
        currentSecondsBeforeDiminish -= Time.deltaTime;
        if (currentSecondsBeforeDiminish > 0)
            return;

        activatateOrDisableLight();
        if (crankingLight)
            return;
        if (player_light.spotAngle > 1)
            diminishLight(lightDiminishPerSecond * Time.deltaTime);
    }

    //Light's spotAngle can't go lower than 1, so if it is 1, then turn the light object off
    private void playerActivateDeactivateLight() {
        if (Input.GetKey(lightSwitching)) {
            lightIsOn = !lightIsOn;
        }
    }

    private void activatateOrDisableLight() {
        player_light.gameObject.SetActive(player_light.spotAngle <= 1 ? false : true);
    }

    public bool lightIsDisabled() {
        return !player_light.gameObject.activeSelf;
    }

    public void increaseLight(float angleAmount) {
        if (player_light.spotAngle < maxLightAngle)
            player_light.spotAngle += angleAmount;
    }

    private void crankLight() {
        if (Input.GetKey(crankKeyPress)) {
            crankingLight = true;
            crankTime = Mathf.Clamp(crankTime += Time.deltaTime, 0, secondsCrankUntilDiminish);
            increaseLight(lightCrankedPerSecond * Time.deltaTime);
        }
        else {
            crankingLight = false;
        }
        if (Input.GetKeyUp(crankKeyPress)) {
            currentSecondsBeforeDiminish = crankTime * secondsCrankUntilDiminish;
            crankTime = 0;
        }
    }

    //For controlling player movement
    private void playerMovement() {
        
        float mov_V = Input.GetAxis("Vertical") * speedMultiplier;
        float mov_H = Input.GetAxis("Horizontal") * speedMultiplier;

        if (mov_V == 0 & mov_H == 0) {
            player_sprite.SetBool("isWalking", false);
            player_rb.velocity = Vector3.zero;
        }
        else if (Mathf.Abs(mov_V) >= Mathf.Abs(mov_H)) {
            player_sprite.SetBool("isWalking", true);
            player_rb.velocity = new Vector3(0, 0, mov_V);
            //transform.Translate(0, 0, mov_V, Space.World);

            if (mov_V > 0)
                player_sprite.SetInteger("Direction", 3);
            else
                player_sprite.SetInteger("Direction", 1);
        }
        else if (Mathf.Abs(mov_V) < Mathf.Abs(mov_H)) {
            player_sprite.SetBool("isWalking", true);
            player_rb.velocity = new Vector3(mov_H, 0, 0);
            //transform.Translate(mov_H, 0, 0, Space.World);

            if (mov_H > 0)
                player_sprite.SetInteger("Direction", 2);
            else
                player_sprite.SetInteger("Direction", 4);
        }

        print("Player Vel: " + player_rb.velocity);
        
    }
	
	// Update is called once per frame
    void Turning ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit cursor;

        if (Physics.Raycast(ray,out cursor,20f,floorMask )) 
        {
            Vector3 player_ray = cursor.point - transform.position;

            player_ray.y = 0.0f;

            Quaternion newRotation = Quaternion.LookRotation(player_ray);

            player_rb.MoveRotation(newRotation);

        }

    }
    //Prototype method for player light casting: Raycasts in a fan, Raycasts trigger enemy behavior
    //It doesn't seem to be working well, though...
    void CastLight ()
    {
        //Only when light is enabled
        if (lightIsDisabled())
            return;
        //To simplify later code
        float angle_radius = player_light.spotAngle / 2;
        float angle_increment = player_light.spotAngle / numberOfRays;
        float light_length = player_light.range;
        //Iterate through angles
        for (float degree = -angle_radius; degree < angle_radius; degree += angle_increment)
        {
            //Code to calculate direction based on degree and Vector3.forward
            Vector3 LightDirection = Quaternion.AngleAxis(degree, Vector3.up) * transform.forward;

            //Cast ray
            RaycastHit hit;
            if(Physics.Raycast(transform.position, LightDirection, out hit, light_length))
            {
                //Trigger enemy behavior
                if (hit.transform.gameObject.tag == "Enemy")
                {
                //    Debug.Log(hit.transform.gameObject.tag);
                    //Get Enemy_Controller script from enemy- will have to change to accept different types of enemies
                    Enemy_Controller enemy = (Enemy_Controller)hit.transform.gameObject.GetComponent(typeof(Enemy_Controller));
                    //Will have to define LightTrigger() method on all enemy scripts with their corresponding response
                    enemy.LightTrigger();
                }
            }
        }

    }

	private float getSpeed () {
		float speed = Time.deltaTime;
		speed *= speedMultiplier;
		speed *= speedKeys();
		return speed;
	}
	private float speedKeys () {
		if (Input.GetAxis ("Run") > 0)
			return 2.0f;
		else if (Input.GetAxis ("Run") < 0)
			return 0.5f;
		else
			return 1f;
	}
}
