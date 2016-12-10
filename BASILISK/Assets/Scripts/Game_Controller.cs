using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class Game_Controller : MonoBehaviour
{
    public static Game_Controller control;
    //    public string pauseButton = "escape";
    public bool paused;
    private GameObject pauseCanvas;
    private Scene CurrentLevel;
    private int level;
    private bool holding_journal;
    private Journal journal_held;

    public int Journal_count;
    //public GameObject[] journals;
    public List<GameObject> journals;
    public GameObject player;
    public List<int> collected;

    public bool contains_journal;
    // Use this for initialization
    void Awake()
    {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        print("started");
        //print(Application.persistentDataPath);
        //print(SceneManager.GetActiveScene().buildIndex);
        paused = false;
        pauseCanvas = transform.GetChild(0).gameObject;
        pauseCanvas.SetActive(false);
        holding_journal = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    collected.Remove(journals[0]);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //   collected.Remove(journals[1]);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //   collected.Remove(journals[2]);
        //}
        //else
        //{
        //print("Invalid input");
        //}
        level = SceneManager.GetActiveScene().buildIndex;
        if (Input.GetKeyDown(KeyCode.I))
        {
            SaveGame();
            //print("saved");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            LoadGame();
            //print("loaded");
        }
        if (Input.GetKeyDown(KeyCode.Y)) { foreach (int thing in collected) { print(thing); } }
        playerPauseGame();
        if (Input.GetKeyDown(KeyCode.R))
            restartLevel();
        runGame();
    }
   
    public void PickUp(Journal J)
    {
        holding_journal = true;
        journal_held = J;
        //loops through all journals, and if they are picked up it adds that journal to a list of journals collected.
        foreach (GameObject journal in journals)
        {
            var distance_to_journal = Vector3.Distance(player.transform.position, journal.transform.position);
            if (distance_to_journal < 2)
            {
                collected.Add(int.Parse(journal.name));
                collected = collected.OrderBy(tile => tile).ToList();
            }

        }
        //Removes specific journal based on which key is pressed, will change to display journal later

    }
    public void restartLevel()
    {
        SceneManager.LoadScene(level);
    }
   
    private void playerPauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            togglePause();
        }
    }
    private void journal_drop()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            holding_journal = false;
            togglePause();
            journal_held.sprite.transform.Translate(0, 0, -.5f);
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            holding_journal = false;
            journal_held.sprite.transform.Translate(0, 0, -.5f);
        }
    }
    public void togglePause() {
        paused = !paused;
        pauseCanvas.SetActive(paused);
    }
    public void SaveGame()
    {
        Journals2 journals_script = GameObject.Find("Journals").GetComponent<Journals2>();
        BinaryFormatter bf = new BinaryFormatter();

        FileStream save = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);
        Saves data = new Saves();
        //Journals2 jc = GameObject.Find("Journals").GetComponent<Journals2>();
        //data.collected = jc.collected;
        data.lvl_id = level;
        data.collected = collected;
        bf.Serialize(save, data);
        save.Close();
    }
    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/save.dat"))
        {
            FileStream save = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            Saves data = (Saves)bf.Deserialize(save);
            save.Close();
            level = data.lvl_id;
            restartLevel();
            collected = data.collected;


        }
        else
        {
            print("does not exist");
        }
    }

    private void runGame()
    {
        journal_drop();
        if (holding_journal)
        {
            Time.timeScale = 0;
        }
        else if (paused && Time.timeScale != 0)
        {
            Time.timeScale = 0;
        }
        else if (!paused && Time.timeScale == 0)
        {
            Time.timeScale = 1;
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
        if (holding_journal == true)
        {
            GUI.Label(new Rect(200, 200, 100, 200), "Press 'F' to close the journal");
        }
    }

    [Serializable]
    class Saves
    {

        public int lvl_id;
        public List<int> collected;
    }


}

// Use this for initialization

