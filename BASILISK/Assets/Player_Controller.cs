using UnityEngine;
using System.Collections;

public class Player_Controller : MonoBehaviour {
    int floorMask;
    Rigidbody player_rb;
    // Use this for initialization
    void Awake () {
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
	
	}
}
