using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLighting : MonoBehaviour {
    private SpriteRenderer[] sprites;
    private int l;
    // Use this for initialization
    void Start () {
        l = 0;
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (l == 0)
            LightOff();
        else if (l == 10)
            LightOn();
        if (l >= 0)
            l--;
    }
    public void LightTrigger()
    {
        l = 10;
    }
    void LightOn() {
        foreach (SpriteRenderer spr in sprites)
        {
            spr.material = Resources.Load("SpriteDefault", typeof(Material)) as Material;
        }
    }
    void LightOff()
    {
        foreach (SpriteRenderer spr in sprites)
        {
            spr.material = Resources.Load("SpriteLit", typeof(Material)) as Material;
        }
    }
}
