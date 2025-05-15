using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour {
	
    void Awake()
    {
        
    }
	// Update is called once per frame
	void Update () {
	
        if(Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
	}

    public void ResetGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
