using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonManagerUI : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;

    public void Play()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void Options()
    {
        panel1.SetActive(true);
        panel2.SetActive(false);
    }

    public void titleScreen()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
    }
    public void TitleScreen()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
