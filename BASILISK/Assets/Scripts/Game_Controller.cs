using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour {

//    public string pauseButton = "escape";
    public bool paused;
    public string sceneName;
    public string firstLevel;
    private GameObject pauseCanvas;

	// Use this for initialization
	void Start () {
        paused = false;

        pauseCanvas = transform.GetChild(0).gameObject;

	}
	
	// Update is called once per frame
	void Update () {
        playerPauseGame();
        
        runGame();
	}

    public void restartLevel() {
        SceneManager.LoadScene(sceneName);
    }

    private void playerPauseGame() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            paused = !paused;
            pauseCanvas.SetActive(!pauseCanvas.activeSelf);
        }
    }
	public void RestartGame ()
	{		
			SceneManager.LoadScene(firstLevel);
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
