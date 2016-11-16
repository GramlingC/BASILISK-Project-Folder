using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour {

	public GameObject player;
    public string nextScene;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Player") 
		{
			victory ();
		}
	}
	void victory(){
        //Application.LoadLevel ("Victory");
        SceneManager.LoadScene(nextScene);

    }
}
