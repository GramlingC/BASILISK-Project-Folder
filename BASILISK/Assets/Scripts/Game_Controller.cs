using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections;

public class Game_Controller : MonoBehaviour {

//    public string pauseButton = "escape";
    public bool paused;
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
        EditorSceneManager.LoadScene("Jeffrey");
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
