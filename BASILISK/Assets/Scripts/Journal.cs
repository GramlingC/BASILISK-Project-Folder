using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
//Will be used to allow player to access previously collected journals when completed
/*
public class Journal_Tracking
{
    public int JournalNumber;
    public bool JournalCollected;
    public string JournalContents;

    public Journal_Tracking(int journalnumber,bool journalcollected,string journalcontents)
    {
        JournalNumber = journalnumber;
        JournalCollected = journalcollected;
        JournalContents = journalcontents;
    }
}

    */
public class Journal : MonoBehaviour {

    public GameObject player;
    private bool journal_held;
    private bool near_journal;
    private bool shouldcontinue;
    public GameObject sprite;
  
    // Use this for initialization
	void Start () {
        journal_held = false;
        near_journal = false;
        shouldcontinue = true;
        
       
    }
    //***Try making an array of GameObjects that contains all of the journals and using a 
    //for loop instead of having to reference each journal individually***
    // Update is called once per frame
    void Update()
    {
        var distance_to_journal = Vector3.Distance(player.transform.position, transform.position);
        if (distance_to_journal < 2)
        {
            near_journal = true;
            Debug.Log(near_journal);
        }
        else
        {
            near_journal = false;
            Debug.Log(near_journal);
        }

        if (distance_to_journal < 2 & Input.GetKeyDown(KeyCode.E))
        {
            transform.Translate(0, -10, 0);
            Journals journals_script = GameObject.Find("Journals").GetComponent<Journals>();
            journals_script.Journal_count = journals_script.Journal_count + 1;
            journal_held = true;
            Debug.Log(journal_held);
        }

    
      
       
            //Closes the journal when the f key is pressed
            
            if (journal_held == true & Input.GetKeyDown(KeyCode.F))
            {
                sprite.transform.Translate(0, 0, -.5f);
                journal_held = false;
                Debug.Log(journal_held);
                Debug.Log(sprite.transform.position);
            }
            //Player picks up and opens journal
            if (near_journal == true & Input.GetKeyDown(KeyCode.E))
            {
                sprite.transform.Translate(0, 0, .5f);
                Debug.Log(sprite.transform.position);
            }
            
        
        
    }
    void OnGUI() {
      
        //Displays "Press E to pick up when player is near journal
        if (near_journal==true)
        {
            GUI.Label(new Rect(10, 10, 100, 200), "Press 'E' to pick up");
        }
        //Displays press f to close journal
        if (journal_held == true)
        {
            GUI.Label(new Rect(200, 200, 100, 200), "Press F to close the journal");
        }
        
         
    }
}
