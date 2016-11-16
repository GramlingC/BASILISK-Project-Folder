using UnityEngine;
using System.Collections;

public class End : MonoBehaviour {

	public GameObject player;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Player") 
		{
			victory ();
		}
	}
	void victory(){
		Application.LoadLevel ("Victory");
	}
}
