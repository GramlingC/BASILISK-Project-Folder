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
		var mov_V = Input.GetAxis("Vertical") * Time.deltaTime * 2.0f;
		var mov_H = Input.GetAxis("Horizontal") * Time.deltaTime * 2.0f;

		transform.Translate(0, 0, mov_V, Space.World);
		transform.Translate(mov_H, 0, 0, Space.World);
	}
}
