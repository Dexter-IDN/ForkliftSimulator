using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRaycast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hitInfo, 20f)) 
        {
            if(hitInfo.collider.tag == "StartButton") {
                if (Input.GetMouseButtonDown(0)) {
                    SceneManager.LoadScene("Simulator");
                }
            } 
        } 
    }
}
