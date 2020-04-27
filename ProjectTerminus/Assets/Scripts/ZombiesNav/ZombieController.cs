using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public Camera cam;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //Create a ray toward the mouse click position
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            // We shoot the ray and we gather information about what we hit
            if(Physics.Raycast(ray, out hit))
            {

            }

        }
    }
}
