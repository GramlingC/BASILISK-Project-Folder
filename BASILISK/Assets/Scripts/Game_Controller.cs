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
	public float number;

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
        level = 0;
		CurrentLevel = SceneManager.GetActiveScene();
        paused = false;
		CurrentLevel = SceneManager.GetActiveScene();
        pauseCanvas = transform.GetChild(0).gameObject;

	}
	
	// Update is called once per frame
	void Update () {
        level = SceneManager.GetActiveScene().buildIndex;
		if (Input.GetKeyDown (KeyCode.K)) {
			SaveGame ();
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			LoadGame ();
		}
        playerPauseGame();
        if (Input.GetKeyDown(KeyCode.R))
            restartLevel();
        runGame();
	}

    public void restartLevel() {
		CurrentLevel = SceneManager.GetActiveScene();
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

		data.lvl_id = level;
		//data.level = CurrentLevel;

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
		//	CurrentLevel = data.level;
		
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
	[Serializable]
	class Saves
	{

		public int lvl_id;
        public List<GameObject> collected;
        //public Object level;
    }


}
