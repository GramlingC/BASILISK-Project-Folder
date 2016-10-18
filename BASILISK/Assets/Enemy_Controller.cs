using UnityEngine;
using System.Collections;

public class Enemy_Controller : MonoBehaviour
{
    private Rigidbody rb;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
       // rb = GetComponent<Rigidbody>();
  //  Vector3 Enemy = new Vector3(-0.5f, 0, 0);
    ///    rb.AddForce(Enemy);

	}
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Light_Hitbox")
        {
            Destroy(gameObject);
        }
    }
}
