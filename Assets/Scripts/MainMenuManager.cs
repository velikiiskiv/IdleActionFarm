using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void Exit()
    {
        Application.Quit();
    }
}