using UnityEngine;
using System.Collections;

public class Journals : MonoBehaviour {
    public int Journal_count;
    
	// Use this for initialization
	void Start () {
        Journal_count = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        GUI.Label(new Rect(200, 10, 100, 200), "Journal Count:");
        GUI.Label(new Rect(286, 11f, 100, 200), Journal_count.ToString());
    }
}
