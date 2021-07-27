using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Menu button manager allows navigation of the 'TitleScene', and
/// //Should allow navigation of 'LobbyScene'. We will need to see if this is feasible.
/// </summary>
public class MenuButtonManagerUI : MonoBehaviour
{
   //Todo: rename these variables

    [SerializeField] [Tooltip("Canvas")] public GameObject panel1;//Necessary?
    [SerializeField] [Tooltip("Panel showing main screen navigation")] public GameObject panel2;

    //Todo: Either have a LoadScene function for use of buttons, or do it through Joes ways of using Lambdas (preferable)

    /// <summary>
    /// Loads LobbyScene
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    /// <summary>
    /// Turns on the Options panel and sets panel2 off
    /// </summary>
    public void Options()
    {
        FindObjectOfType<AudioManager>().TurnOn();
        panel2.SetActive(false);
    }

    /// <summary>
    /// Goes back to the root navigation screen and sets Options panel off
    /// </summary>
    public void titleScreen()
    {
        FindObjectOfType<AudioManager>().TurnOff();
        panel2.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
