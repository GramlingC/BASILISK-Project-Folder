using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public Transform player;
    // Use this for initialization
    Vector3 offset;
    void Start () {
        offset = transform.position - player.position;
	}
	
	// Update is called once per frame
	void Update () {
        // Create a postion the camera is aiming for based on the offset from the target.
        Vector3 playerCamPos = player.position + offset;
        // Smoothly interpolate between the camera's current position and it's target position.
        transform.position = Vector3.Lerp(transform.position, playerCamPos, 100f * Time.deltaTime);
    }
}
