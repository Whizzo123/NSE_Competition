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
        FindObjectOfType<AudioManager>().TurnOn();
        panel2.SetActive(false);
    }

    public void titleScreen()
    {
        FindObjectOfType<AudioManager>().TurnOff();
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
