using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Journals : MonoBehaviour {
    public int Journal_count;
    public GameObject[] journals;
    public GameObject player;
    public List<GameObject> collected;
    public int number;
    public bool contains_journal;
	// Use this for initialization
	void Start () {
        Journal_count = 0;
        collected = new List<GameObject>();

    }
	
	// Update is called once per frame
	void Update () {
        
        //loops through all journals, and if they are picked up it adds that journal to a list of journals collected.
	foreach(GameObject journal in journals)
        {
            var distance_to_journal = Vector3.Distance(player.transform.position,journal.transform.position);
            if (distance_to_journal < 2 & Input.GetKeyDown(KeyCode.E))
            {
                collected.Add(journal);
                collected = collected.OrderBy(tile => tile.name).ToList();
            }
           if (collected.Contains(journals[number])){
                print("contains journal");
            }
        }
    //Removes specific journal based on which key is pressed, will change to display journal later
    if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            collected.Remove(journals[0]);
        }
    else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            collected.Remove(journals[1]);
        }
    else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            collected.Remove(journals[2]);
        }
    else
        {
            print("Invalid input");
        }
	}

    void OnGUI()
    {
        GUI.Label(new Rect(200, 10, 100, 200), "Journal Count:");
        GUI.Label(new Rect(286, 11f, 100, 200), Journal_count.ToString());
        if (Input.GetKey(KeyCode.K))
        {
            GUI.Label(new Rect(300, 200, 100, 200), "Press number to select journal");
        }
    }
}
