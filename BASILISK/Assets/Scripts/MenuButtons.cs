﻿using UnityEngine;
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
        SceneManager.LoadScene("Level1");
    }

    public void loadScene2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void loadScene3()
    {
        SceneManager.LoadScene("Level3");
    }

    public void loadScene4()
    {
        SceneManager.LoadScene("Level4");
    }

    public void loadPlaytesting() {
        SceneManager.LoadScene("Playtesting level");
    }

    public void loadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void quit()
    {
        Application.Quit();
    }
}
