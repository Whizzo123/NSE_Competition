using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackTemp : MonoBehaviour
{
    public void Back()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
