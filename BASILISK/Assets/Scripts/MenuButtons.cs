using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public void loadTutorial() {
        SceneManager.LoadScene("Tutorial");
    }
    
    public void loadScene1()
    {
        SceneManager.LoadScene("Scene1");
    }

    public void loadPlaytesting() {
        SceneManager.LoadScene("Playtesting level");
    }

    public void loadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

}
