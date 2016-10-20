using UnityEngine;
using System.Collections;

public class Player_Controller : MonoBehaviour {
    
	public float speedMultiplier;

	int floorMask;
    Rigidbody player_rb;
    // Use this for initialization
	void Start () {
	}
    void Awake () {
		speedMultiplier = 2f;
        floorMask = LayerMask.GetMask("Floor");
        player_rb = GetComponent<Rigidbody>();
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
	void Update () {
		var mov_V = Input.GetAxis("Vertical") * getSpeed();
		var mov_H = Input.GetAxis("Horizontal") * getSpeed();

		transform.Translate(0, 0, mov_V, Space.World);
		transform.Translate(mov_H, 0, 0, Space.World);
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
