using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class DialogueController : MonoBehaviour {
    public MonoBehaviour[] targets;
    public string dialogue = "";
    public int distance = 6;
    public bool obscurable = false;
    public bool started = false;
    public bool finished = false;
    public int speed = 4;
    
    public int dialogue_type; //0 for regular dialogue; 1 for randomly assigned dialogue

    private List<string> names;
    StreamReader sr;
    private string lines;

    // Use this for initialization
    void Start() {
    }
    
    IEnumerator Text(StreamReader sr)
    {
        if (dialogue_type == 0)
        {
            do
            {
                string line = sr.ReadLine();
                string[] segments = line.Split(':');
                if (segments.Length > 1)
                {
                    MonoBehaviour target;
                    if (names.Count > 0 && names.Exists(x => x.Equals(segments[0])))
                    {
                        target = targets[names.IndexOf(segments[0])];
                    }
                    else
                    {
                        names.Add(segments[0]);
                        target = targets[names.Count - 1];
                    }
                    DialogueLabel label = gameObject.AddComponent<DialogueLabel>() as DialogueLabel;
                    label.message = segments[1];
                    label.target = target;
                    label.obscurable = obscurable;
                    label.distance = distance;
                    yield return new WaitForSeconds(2.0f + 0.12f * segments[1].Length * (1/speed));
                    Destroy(label.text);
                    Destroy(label);
                }
            } while (!sr.EndOfStream && !targetsAlerted());
        }
        else
        {
            string readline = sr.ReadToEnd();
            string[] lines = readline.Split('\n');
            string randomline = lines[(int)Random.Range(0f, lines.Length + 0.99f)];
            MonoBehaviour target = targets[0];
            DialogueLabel label = gameObject.AddComponent<DialogueLabel>() as DialogueLabel;
            label.message = randomline;
            label.target = target;
            label.obscurable = obscurable;
            label.distance = distance;
            yield return new WaitForSeconds(1.0f + 0.08f * randomline.Length);
            Destroy(label.text);
            Destroy(label);

        }
        finished = true;
    }

    bool targetsAlerted ()
    {
        int i = 0;
        do
        {
            if (targets[i].GetType() == typeof(Guard_Controller_v2))
            {
                Guard_Controller_v2 target = targets[i].gameObject.GetComponent<Guard_Controller_v2>() as Guard_Controller_v2;
                if (target.detectsPlayer())
                {
                    return true;
                }
            }
            i++;
        }
        while (i < targets.Length);
        return false;
    }

	// Update is called once per frame
	void Update ()
    {
        if (!dialogue.Equals("") && !started)
        {
            started = true;
            sr = new StreamReader("Assets\\Text\\" + dialogue + ".txt");
            names = new List<string>();
            Coroutine routine = StartCoroutine(Text(sr));
        }
    }
}
