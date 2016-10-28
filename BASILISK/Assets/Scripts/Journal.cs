using UnityEngine;
using System.Collections;

public class Journal : MonoBehaviour {

    public GameObject player;
    public int Journal_Count;
   
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var distance_to_journal = Vector3.Distance(player.transform.position, transform.position);
        if (distance_to_journal < 2 && Input.GetKeyDown(KeyCode.E))
        {
            Journal_Count = Journal_Count + 1;
            transform.Translate(0, -10, 0);
        }

    }
    void OnGUI() {
        var distance_to_journal = Vector3.Distance(player.transform.position, transform.position);
        if (distance_to_journal < 2)
        {
            GUI.Label(new Rect(10, 10, 100, 200), "Press 'E' to pick up");
        }
        GUI.Label(new Rect(200, 10, 100, 200), "Journal Count:");
        GUI.Label(new Rect(286, 11f, 100, 200), Journal_Count.ToString());
    }
}
