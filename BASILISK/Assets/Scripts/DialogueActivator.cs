﻿using UnityEngine;
using System.Collections;

public class DialogueActivator : MonoBehaviour
{

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            DialogueController script = (DialogueController)GetComponent(typeof(DialogueController));
            script.enabled = true;
        }
    }
}
