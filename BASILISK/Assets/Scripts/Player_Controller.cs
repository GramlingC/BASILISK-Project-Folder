using UnityEngine;
using System.Collections;

public class Player_Controller : MonoBehaviour {
    
	public float speedMultiplier;
    public int numberOfRays = 10;

	int floorMask;
    Rigidbody player_rb;
    Light player_light;
    public float lightDiminishPerSecond = 5f;
    // Use this for initialization
    void Start () {
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
        var mov_V = Input.GetAxis("Vertical") * getSpeed();
        var mov_H = Input.GetAxis("Horizontal") * getSpeed();

        transform.Translate(0, 0, mov_V, Space.World);
        transform.Translate(mov_H, 0, 0, Space.World);

        //Call CastLight
        CastLight();
        gradualLightDiminish();
    }

    //Functions dealing wih diminishing light. Public functions are so that other scripts can call it also
    public void diminishLight(float angleAmount) {
        player_light.spotAngle -= angleAmount;
    }

    private void gradualLightDiminish() {
        if (player_light.spotAngle > 1)
            diminishLight(lightDiminishPerSecond * Time.deltaTime);
        disableLight();
    }

    //Light's spotAngle can't go lower than 1, so if it is 1, then turn the light object off
    private void disableLight() {
        player_light.gameObject.SetActive(player_light.spotAngle <= 1 ? false : true);
    }

    public bool lightIsDisabled() {
        return !player_light.gameObject.activeSelf;
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
                    Debug.Log(hit.transform.gameObject.tag);
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
