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
        if(count >= limit) {
            Debug.Log("Game Over");

            Time.timeScale = 0;
          
            SceneManager.LoadScene("GameFinish");
        }
    }


}
