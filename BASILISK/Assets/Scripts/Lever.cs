using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {

	public GameObject Door;
	private End door_script;
	// Use this for initialization
	void Start () {
		door_script = Door.GetComponent<End> ();
	}
	
	// Update is called once per frame
	void Update () {
			
	}
	void OnTriggerEnter (Collider collider)
	{
		if (collider.gameObject.tag == "Player")
			door_script.islock = false;

			
	

	}
} 
