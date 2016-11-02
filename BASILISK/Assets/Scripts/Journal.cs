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
    public int Journal_Count;
    private bool journal_held;
    private bool near_journal;
    private bool shouldcontinue;
    public GameObject[] journal_sprite_list;
    public GameObject[] journal_object_list;
  
    // Use this for initialization
	void Start () {
       
        Journal_Count = 0;
        journal_held = false;
        near_journal = false;
        shouldcontinue = true;
        
       
    }
    //***Try making an array of GameObjects that contains all of the journals and using a 
    //for loop instead of having to reference each journal individually***
    // Update is called once per frame
    void Update() {
            foreach (GameObject journal in journal_object_list)
            {
                var distance_to_journal = Vector3.Distance(player.transform.position, journal.transform.position);
                if (distance_to_journal < 2)
                {
                    near_journal = true;
                }
                if (distance_to_journal > 2)
                {
                    near_journal = false;
                }

                if (distance_to_journal < 2 & Input.GetKeyDown(KeyCode.E))
                {
                    journal.transform.Translate(0, -10, 0);
                    Journal_Count = Journal_Count + 1;
                    journal_held = true;
                }

            }
      
        foreach (GameObject sprite in journal_sprite_list)
        {
            //Closes the journal when the f key is pressed
            
            if (journal_held == true & Input.GetKeyDown(KeyCode.F))
            {
                sprite.transform.Translate(0, 0, -.5f);
                journal_held = false;
                Debug.Log(sprite.transform.position);
            }
            //Player picks up and opens journal
            if (near_journal == true & Input.GetKeyDown(KeyCode.E))
            {
                sprite.transform.Translate(0, 0, .5f);
                Debug.Log(sprite.transform.position);
            }
            
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
        
            //Displays Journal count
            GUI.Label(new Rect(200, 10, 100, 200), "Journal Count:");
        GUI.Label(new Rect(286, 11f, 100, 200), Journal_Count.ToString());
    }
}
