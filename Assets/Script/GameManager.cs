using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int LIMIT;
    public static int COUNT;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void LateUpdate()
    {
        // Debug.Log("count : " + COUNT);
        // Debug.Log("limit : " + LIMIT);

        if(COUNT >= LIMIT) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene("GameFinish");
        }
    }
}
