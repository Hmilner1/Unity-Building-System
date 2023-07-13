using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneMan : MonoBehaviour
{

    public void OnClickPlay()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnClickExit()
    { 
        Application.Quit();
    }
}
