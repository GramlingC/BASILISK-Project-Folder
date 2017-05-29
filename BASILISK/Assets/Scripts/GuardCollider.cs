using UnityEngine;
using System.Collections.Generic;

public class GuardCollider : MonoBehaviour
{
    private Guard_Controller_v2 script;

    void Start()
    {
        GameObject parent = transform.parent.gameObject;
        script = (Guard_Controller_v2)parent.GetComponentInChildren(typeof(Guard_Controller_v2),true);
    }
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag != "Bat")
            return;
        Vector3 LightDirection = col.transform.position - transform.position;
        RaycastHit hit;
        Debug.DrawRay(transform.position, LightDirection, Color.green, 6);
        if (Physics.Raycast(transform.position, LightDirection, out hit, 6))
        {
            if (hit.transform.gameObject.tag == "Bat")
                script.BatStay(col);
        }
            
    }
    void OnTriggerExit(Collider col)
    {
        script.BatExit(col);
    }

}
