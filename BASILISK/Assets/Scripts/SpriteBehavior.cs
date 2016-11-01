using UnityEngine;
using System.Collections;

public class SpriteBehavior : MonoBehaviour 
{
	public Transform target;
	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().shadowCastingMode =  UnityEngine.Rendering.ShadowCastingMode.On;
		GetComponent<Renderer>().receiveShadows = true;
	}

	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(target.position.x,transform.position.y, target.position.z);
	}
}
