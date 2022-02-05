using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Menu button manager allows navigation of the 'TitleScene', and
/// //Should allow navigation of 'Matchmaking'. We will need to see if this is feasible.
/// </summary>
public class MenuButtonManagerUI : MonoBehaviour
{
    public GameObject canvas;
    [SerializeField] [Tooltip("Panel showing main screen navigation")] public GameObject mainScreen;
    public string versionNumber = "V0.2.2a";
    private Text whiteVersionNumber, blackVersionNumber;
    //Todo: Either have a LoadScene function for use of buttons, or do it through Joes ways of using Lambdas (preferable)

    private void Start()
    {
        if (mainScreen == null)
        {
            mainScreen = GameObject.Find("MainMenuScreen");
        }
        whiteVersionNumber = GameObject.Find("W_VersionNumberText").GetComponent<Text>();
        blackVersionNumber = GameObject.Find("B_VersionNumberText").GetComponent<Text>();
        whiteVersionNumber.text = versionNumber;
        blackVersionNumber.text = versionNumber;
    }
    /// <summary>
    /// Loads Matchmaking
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene("Matchmaking");
    }

    /// <summary>
    /// Turns on the Options panel and sets panel2 off
    /// </summary>
    public void Options(bool on)
    {
        GameObject.FindObjectOfType<AudioManager>().PanelSettings(on);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
