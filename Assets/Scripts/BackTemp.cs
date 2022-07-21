using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public enum ActiveScreen { CREATE, BROWSE, LOBBY};
public class BackTemp : MonoBehaviour
{
    private Dictionary<ActiveScreen, bool> currentScreen;


    private void Awake()
    {
        currentScreen = new Dictionary<ActiveScreen, bool>();
        currentScreen.Add(ActiveScreen.CREATE, true);
        currentScreen.Add(ActiveScreen.BROWSE, false);
        currentScreen.Add(ActiveScreen.LOBBY, false);
    }

    public void SwitchingTo(ActiveScreen screenSwitchingTo)
    {
        currentScreen[ActiveScreen.CREATE] = false;
        currentScreen[ActiveScreen.BROWSE] = false;
        currentScreen[ActiveScreen.LOBBY] = false;

        currentScreen[screenSwitchingTo] = true;
    }

    public void Back()
    {
        
        if(currentScreen[ActiveScreen.CREATE])
        {
            Debug.Log("On Create screen");
            BackToTitle();
        }
        else if(currentScreen[ActiveScreen.BROWSE])
        {
            Debug.Log("On Browse screen");
            BackToCreate();
        }
        else if(currentScreen[ActiveScreen.LOBBY])
        {
            Debug.Log("On Lobby screen");
            MyNetworkManager.singleton.StopClient();
            MyNetworkManager.singleton.StopServer();
            BackToCreate();
        }
    }

    private void BackToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    private void BackToCreate()
    {
        FindObjectOfType<LobbyUIManager>().ChangeScreenTo("Create");
    }
}
