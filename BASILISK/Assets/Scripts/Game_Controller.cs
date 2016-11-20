using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class Game_Controller : MonoBehaviour {
	public static Game_Controller control;
//    public string pauseButton = "escape";
    public bool paused;
    private GameObject pauseCanvas;
	private Scene CurrentLevel;
    private int level;

    public int Journal_count;
    public GameObject[] journals;
    public GameObject player;
    public List<GameObject> collected;
    public int number;
    public bool contains_journal;

    // Use this for initialization
    //void Awake(){
    //if (control == null) 
    //{
    //DontDestroyOnLoad (gameObject);
    //control = this;
    //}
    //else if(control != this)
    //{
    //Destroy (gameObject);
    //}
    //}
    void Start () {
        print(Application.persistentDataPath);
        print(SceneManager.GetActiveScene().buildIndex);
        level = 0;
		CurrentLevel = SceneManager.GetActiveScene();
        paused = false;
		CurrentLevel = SceneManager.GetActiveScene();
        pauseCanvas = transform.GetChild(0).gameObject;

    }
	
	// Update is called once per frame
	void Update () {
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
		if (Input.GetKeyDown (KeyCode.I)) {
			SaveGame ();
            print("saved");
		}
		if (Input.GetKeyDown (KeyCode.O)) {
            print("loaded");
            LoadGame ();
            restartLevel();
		}
        playerPauseGame();
        if (Input.GetKeyDown(KeyCode.R))
            restartLevel();
        runGame();
	}
    public void PickUp()
    {

        //loops through all journals, and if they are picked up it adds that journal to a list of journals collected.
        foreach (GameObject journal in journals)
        {
            var distance_to_journal = Vector3.Distance(player.transform.position, journal.transform.position);
            if (distance_to_journal < 2)
            {
                print("about to add journal");
                collected.Add(journal);
                print("added journal");
                collected = collected.OrderBy(tile => tile.name).ToList();
            }

        }
        //Removes specific journal based on which key is pressed, will change to display journal later

    }
    public void restartLevel() {
		SceneManager.LoadScene(level);

    }

    private void playerPauseGame() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            paused = !paused;
            pauseCanvas.SetActive(!pauseCanvas.activeSelf);
        }
    }
	public void SaveGame()
	{
		Journals2 journals_script = GameObject.Find("Journals").GetComponent<Journals2>();
		BinaryFormatter bf = new BinaryFormatter();

		FileStream save = File.Open (Application.persistentDataPath + "/save.dat", FileMode.Open);
		Saves data = new Saves();
        //Journals2 jc = GameObject.Find("Journals").GetComponent<Journals2>();
        //data.collected = jc.collected;
        data.lvl_id = level;

		bf.Serialize (save, data);
		save.Close ();
	}
	public void LoadGame ()
	{	
		if (File.Exists (Application.persistentDataPath + "/save.dat")) {
			FileStream save = File.Open(Application.persistentDataPath + "/save.dat",FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            Saves data = (Saves)bf.Deserialize(save);
			save.Close ();
			level = data.lvl_id;
            //Journals2 jc = GameObject.Find("Journals").GetComponent<Journals2>();
            //jc.collected = data.collected;
		
		} 
		else 
		{
			print ("does not exist");
		}
	}

    private void runGame() {
        if (paused && Time.timeScale != 0) {
            Time.timeScale = 0;
        }
        else if (!paused && Time.timeScale == 0) {
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
    }

    [Serializable]
	class Saves
	{

		public int lvl_id;
        public List<GameObject> collected;
        //public Object level;
    }


}
