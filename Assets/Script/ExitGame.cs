using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void ExitGameButton() {
        Application.Quit();

        Debug.Log("Game is exiting");
    }
}
