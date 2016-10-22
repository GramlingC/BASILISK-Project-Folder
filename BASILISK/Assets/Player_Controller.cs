using UnityEngine;
using System.Collections;

public class Player_Controller : MonoBehaviour {
    
	public float speedMultiplier;

	int floorMask;
    Rigidbody player_rb;
    Light player_light;
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
        //To simplify later code
        float angle_radius = player_light.spotAngle / 2;
        float angle_increment = player_light.spotAngle / 10;
        float light_length = player_light.range;
        //Iterate through angles
        for (float degree = -angle_radius; degree < angle_radius; degree += angle_increment)
        {
            //Code to calculate direction based on degree and Vector3.forward
            Vector3 LightDirection = Quaternion.AngleAxis(degree, Vector3.up) * transform.forward;

            RaycastHit hit;
            if(Physics.Raycast(transform.position, LightDirection, out hit, light_length))
            {
                //Debug.Log(hit.transform.gameObject.tag);
                if (hit.transform.gameObject.tag == "Enemy")
                {
                    //Trigger enemy behavior
                    Debug.Log(hit.transform.gameObject.tag);
                }
            }
        }

    }
	void Update () {
		var mov_V = Input.GetAxis("Vertical") * getSpeed();
		var mov_H = Input.GetAxis("Horizontal") * getSpeed();

		transform.Translate(0, 0, mov_V, Space.World);
		transform.Translate(mov_H, 0, 0, Space.World);

        //Uncomment method call if you get it to work
        //CastLight();
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
