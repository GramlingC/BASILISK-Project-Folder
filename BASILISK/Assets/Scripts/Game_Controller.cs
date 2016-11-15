using UnityEngine;
using System.Collections;

public class Game_Controller : MonoBehaviour {

//    public string pauseButton = "escape";
    public bool paused;
    private GameObject pauseCanvas;

	// Use this for initialization
	void Start () {
        paused = false;
        pauseCanvas = GameObject.Find("PauseMenu");
        pauseCanvas.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        playerPauseGame();

        runGame();
	}

    private void playerPauseGame() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            paused = !paused;
            pauseCanvas.SetActive(!pauseCanvas.activeSelf);
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

}
