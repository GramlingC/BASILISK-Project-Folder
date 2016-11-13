﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class DialogueController : MonoBehaviour {
    public MonoBehaviour[] targets;
    public string dialogue;

    private List<string> names;
    StreamReader sr;
    private string lines;
    // Use this for initialization
    void Start() {
        sr = new StreamReader("Assets\\Text\\" + dialogue + ".txt");
        names = new List<string>();
        Coroutine routine = StartCoroutine(Text(sr));
    }
    
    IEnumerator Text(StreamReader sr)
    {
        do
        {
            string line = sr.ReadLine();
            string [] segments = line.Split(':');
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
                yield return new WaitForSeconds(1.0f + 0.08f * segments[1].Length);
                Destroy(label);
            }
        } while (!sr.EndOfStream && !targetsAlerted());
    }

    bool targetsAlerted ()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].GetType() == typeof(Enemy_Controller))
            {
                Enemy_Controller target = targets[i].gameObject.GetComponent<Enemy_Controller>() as Enemy_Controller;
                if (target.canSeePlayer)
                {
                    return true;
                }
            }
        }
        return false;
    }

	// Update is called once per frame
	void Update () {
    }
}
