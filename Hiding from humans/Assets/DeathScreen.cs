using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DeathScreen : MonoBehaviour
{

    public void SwitchToMainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("mainMenu");
    }

    

}
