using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
//Will be used to allow player to access previously collected journals when completed
  
public class Journal : MonoBehaviour {

    private GameObject Text;
    public GameObject player;
    private bool journal_held;
    private bool near_journal;
    public GameObject sprite;
    private GameObject journal;
    private Game_Controller journals_script;
  
    // Use this for initialization
	void Start () {
        Text = GameObject.Find("TextCanvas/PickUp");
        journal = gameObject;
        journals_script = GameObject.Find("GameController").GetComponent<Game_Controller>();

        journals_script.journals.Add(journal);
        journals_script.journals.RemoveAll(item => item == null);        
        journal_held = false;
        near_journal = false;        
       
    }
    //***Try making an array of GameObjects that contains all of the journals and using a 
    //for loop instead of having to reference each journal individually***
    // Update is called once per frame
    void Update()
    {
        var distance_to_journal = Vector3.Distance(player.transform.position, transform.position);
        if (distance_to_journal < 2)
        {
            Text.GetComponent<Text>().enabled = true;
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            Text.GetComponent<RectTransform>().position = new Vector3(pos.x,pos.y + 20, pos.z);
            near_journal = true;
        }
        else
        {
            if (distance_to_journal < 3)
                Text.GetComponent<Text>().enabled = false;
            near_journal = false;
        }

        if (distance_to_journal < 2 & Input.GetKeyDown(KeyCode.E))
        {
            journals_script.Journal_count = journals_script.Journal_count + 1;
            sprite.transform.Translate(0, 0, .5f);
            transform.Translate(0, -10, 0);
            journal_held = true;
            journals_script.PickUp(this);
            Text.GetComponent<Text>().enabled = false;
        }
            
        
        
    }
    public void displayjournal()
    {
        sprite.transform.Translate(0, 0, .5f);
        journals_script.holding_journal = true;
    }
    public void hidejournal()
    {
        sprite.transform.Translate(0, 0, -.5f);
        journals_script.holding_journal = false;
    }

}
