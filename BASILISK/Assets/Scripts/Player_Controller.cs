using UnityEngine;
using System.Collections;

public class Player_Controller : MonoBehaviour {
    
	public float speedMultiplier;
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

    
    // Use this for initialization
    void Start () {
        lightIsOn = true;
        crankingLight = false;
        currentSecondsBeforeDiminish = secondsUntilStartDiminish;
        crankTime = 0;
        secondsCrankUntilDiminish = 1;
	}
    void Awake () {
		speedMultiplier = 2f;
        floorMask = LayerMask.GetMask("Floor");
        player_rb = GetComponent<Rigidbody>();
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
        
        var mov_V = Input.GetAxis("Vertical") * getSpeed();
        var mov_H = Input.GetAxis("Horizontal") * getSpeed();

        if (mov_V == 0 & mov_H == 0)
            player_sprite.SetBool("isWalking", false);

        else if (Mathf.Abs(mov_V) >= Mathf.Abs(mov_H))
        {
            player_sprite.SetBool("isWalking", true);
            transform.Translate(0, 0, mov_V, Space.World);

            if (mov_V > 0)
                player_sprite.SetInteger("Direction", 3);
            else
                player_sprite.SetInteger("Direction", 1);
        }
        else if (Mathf.Abs(mov_V) < Mathf.Abs(mov_H))
        {
            player_sprite.SetBool("isWalking", true);
            transform.Translate(mov_H, 0, 0, Space.World);
            
            if (mov_H > 0)
                player_sprite.SetInteger("Direction", 2);
            else
                player_sprite.SetInteger("Direction", 4);
        }
        
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
                if (hit.transform.gameObject.tag == "Guard")
                {
                    Guard_Controller_v2 enemy = (Guard_Controller_v2)hit.transform.gameObject.GetComponent(typeof(Guard_Controller_v2));
                    enemy.LightTrigger();
                }
                else if (hit.transform.gameObject.tag == "Bat")
                {
                    Bat_Controller enemy = (Bat_Controller)hit.transform.gameObject.GetComponent(typeof(Bat_Controller));
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
