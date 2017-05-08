using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pirate : MonoBehaviour {

    // Use this for initialization
    public string type;
    public int health;
    public int damage;
    public int moves;
    private bool exhausted;
    public Camera camera;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //void OnGUI()
    //{
        //Vector3 position = camera.WorldToScreenPoint(transform.position);
        
        //GUI.Label(new Rect(,500,100,20), type);
    //}
}
