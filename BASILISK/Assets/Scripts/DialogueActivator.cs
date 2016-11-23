using UnityEngine;
using System.Collections;

public class DialogueActivator : MonoBehaviour
{
    public MonoBehaviour[] targets;
    public string dialogue;
    public int distance;
    public bool obscurable;
    public int dialogue_type; //0 for regular dialogue; 1 for randomly assigned dialogue

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.gameObject.tag);
        if (col.gameObject.tag == "Player")
        {
            DialogueController script = (DialogueController)GetComponent(typeof(DialogueController));
            script.enabled = true;
            script.targets = targets;
            script.dialogue = dialogue;
            script.dialogue_type = dialogue_type;
            script.obscurable = obscurable;
            script.distance = distance;
        }
    }
}
