using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour {
    public GameObject A;
	// Use this for initialization
	void Start () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            A.SetActive(true); 
        }
    }
}
