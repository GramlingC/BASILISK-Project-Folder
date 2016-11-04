using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Journals : MonoBehaviour {
    public int Journal_count;
    public GameObject[] journals;
    public GameObject player;
    public List<GameObject> collected;
	// Use this for initialization
	void Start () {
        Journal_count = 0;
        collected = new List<GameObject>();

    }
	
	// Update is called once per frame
	void Update () {
	foreach(GameObject journal in journals)
        {
            var distance_to_journal = Vector3.Distance(player.transform.position,journal.transform.position);
            if (distance_to_journal < 2 & Input.GetKeyDown(KeyCode.E))
            {
                collected.Add(journal);
                Debug.Log(collected);
              
            }
           if (collected.Count == 3)
            {
                print("success");
            }
        }
	}

    void OnGUI()
    {
        GUI.Label(new Rect(200, 10, 100, 200), "Journal Count:");
        GUI.Label(new Rect(286, 11f, 100, 200), Journal_count.ToString());
    }
}
