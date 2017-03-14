using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueLabel : MonoBehaviour {
    public string message;
    //GUIStyle style;
    private GameObject canvas;
    public GameObject text;
    protected GameObject player;
    public MonoBehaviour target;
    public int distance;
    public bool obscurable;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = GameObject.Find("TextCanvas");
        text = new GameObject();
        text.transform.parent = canvas.transform;
        text.AddComponent<RectTransform>();
        text.AddComponent<Text>();
        RectTransform childRectTransform = text.GetComponent<RectTransform>();
        //RectTransform r = GameObject.Find("TextCanvas/PickUp").GetComponent<RectTransform>();
        //childRectTransform.position = r;
        childRectTransform.sizeDelta = new Vector2(160,30);
        Text textComponent = text.GetComponent<Text>();
        textComponent.font = GameObject.Find("TextCanvas/PickUp").GetComponent<Text>().font;
        textComponent.text = message;
        textComponent.color = new Color(1,1,1,1);
        textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
        textComponent.verticalOverflow = VerticalWrapMode.Truncate;
        textComponent.alignment = TextAnchor.UpperCenter;
        textComponent.alignByGeometry = true;
        textComponent.resizeTextForBestFit = true;

    }

    // Update is called once per frame
    void Update () {
        Vector3 pos = Camera.main.WorldToScreenPoint(target.transform.position);
        text.GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y + 50, pos.z);

    }
    
    bool withinDistance (int distance)
    {
        float player_distance = Vector3.Distance(player.transform.position, target.transform.position);
        return player_distance < distance;
    }
    //~DialogueLabel()
    //{
     //   Destroy(text);
    //}
}
