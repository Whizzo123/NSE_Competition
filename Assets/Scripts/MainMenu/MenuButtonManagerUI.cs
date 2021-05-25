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

    /// <summary>
    /// ////////////////////////////////////////////Remove this(When safe), it's only used in back button for the lobby scene, which needs to use bolt not mono
    /// </summary>
    public void TitleScreen()
    {
        SceneManager.LoadScene("TitleScene");

        //BoltNetwork should be destroyed here. However this is also used in the title screen, where boltnetwork is not active.
    }

    public void Quit()
    {
        Application.Quit();
    }

}
