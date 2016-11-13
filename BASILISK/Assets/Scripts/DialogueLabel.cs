using UnityEngine;
using System.Collections;

public class DialogueLabel : MonoBehaviour {
    public string message;
    GUIStyle style;
    protected GameObject player;
    public MonoBehaviour target;

    // Use this for initialization

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        style = new GUIStyle();

        style.fontSize = 16;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
    }

    void OnGUI () {
        var point = Camera.main.WorldToScreenPoint(target.transform.position);
        if (withinDistance (4))
            GUI.Label(new Rect(point.x - 4 * message.Length, Screen.currentResolution.height - point.y - 600, 90, 150), message, style);
        //GUI.Label(new Rect(point.x - 4 * message.Length, Screen.currentResolution.height - point.y - 600, 90, 150), obscureMessage(message), style);
    }

    // Update is called once per frame
    void Update () {
    }
    
    string obscureMessage (string message)
    {
        float player_distance = Vector3.Distance(player.transform.position, target.transform.position);
        char[] newMessage = message.ToCharArray();
        for (int i = 0; i < message.Length; i++)
        {
            if (Random.value > (3.5 / player_distance))
            {
                if (Random.value > 0.4)
                    newMessage[i] = '.';
                else if (Random.value > 0.6)
                    newMessage[i] = 'm';
                else
                    newMessage[i] = ' ';
            }
        }
        return new string(newMessage);
    }

    bool withinDistance (int distance)
    {
        float player_distance = Vector3.Distance(player.transform.position, target.transform.position);
        return player_distance < distance;
    }
}
