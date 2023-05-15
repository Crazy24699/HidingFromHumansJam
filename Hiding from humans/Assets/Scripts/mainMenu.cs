using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("Main Level");
    }

    public void startDefault()
    {
        SceneManager.LoadScene("easyMode");
    }
    public void startEasy()
    {
        SceneManager.LoadScene("easyMode");
    }
    public void startMedium()
    {
        SceneManager.LoadScene("mediumMode");
    }
    public void startHard()
    {
        SceneManager.LoadScene("hardMode");
    }
}
