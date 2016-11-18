using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour {
	public static Game_Controller control;
//    public string pauseButton = "escape";
    public bool paused;
    private GameObject pauseCanvas;
	private Scene CurrentLevel;

	// Use this for initialization
	void Awake(){
		if (control == null) 
		{
			DontDestroyOnLoad (gameObject);
			control = this;
		}
		else if(control != this)
		{
			Destroy (gameObject);
		}
	}
	void Start () {
		CurrentLevel = SceneManager.GetActiveScene();
        paused = false;
		Journals2 journals_script = GameObject.Find("Journals").GetComponent<Journals2>();
		//journals_script = journals_script.collected<>();
		CurrentLevel = SceneManager.GetActiveScene();
        pauseCanvas = transform.GetChild(0).gameObject;

	}
	
	// Update is called once per frame
	void Update () {
        playerPauseGame();
        if (Input.GetKeyDown(KeyCode.R))
            restartLevel();
        runGame();
	}

    public void restartLevel() {
		CurrentLevel = SceneManager.GetActiveScene();
		SceneManager.LoadScene(CurrentLevel.name);

    }

    private void playerPauseGame() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            paused = !paused;
            pauseCanvas.SetActive(!pauseCanvas.activeSelf);
        }
    }
	public void SaveGame()
	{
	
	}
	public void RestartGame ()
	{	
		
	}

    private void runGame() {
        if (paused && Time.timeScale != 0) {
            Time.timeScale = 0;
        }
        else if (!paused && Time.timeScale == 0) {
            Time.timeScale = 1;
        }
    }

}
