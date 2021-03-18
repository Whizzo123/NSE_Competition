using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonManagerUI : MonoBehaviour
{
    
    public void Play()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
