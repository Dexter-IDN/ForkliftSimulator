using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int limit;
    public static int count;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void LateUpdate()
    {
        // Debug.Log("count : " + count);
        // Debug.Log("limit : " + limit);

        if(count >= limit) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene("GameFinish");
        }
    }
}
