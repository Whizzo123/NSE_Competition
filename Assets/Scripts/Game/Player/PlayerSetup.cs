using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour
{
    
    public void StartAuthority(PlayerController controller)
    {
        //Attatches Camera
        controller.vCam = FindObjectOfType<Cinemachine.CinemachineFreeLook>();
        if (controller.vCam != null)
        {
            Invoke("SetCamera", 1);
        }
        else
        {
            Invoke("SetCamera", 5);
        }

        //Setup player components and immobolise player
        CmdSetupPlayer(controller);
        controller.SetLoadoutReleased(false);
    }

    /// <summary>
    /// Resets some variables and sets up some components
    /// </summary>
    [Command]
    private void CmdSetupPlayer(PlayerController controller)
    {
        //Components
        controller.abilityInventory = new AbilityInventory(controller);
        controller.SetArtefactInventory(GetComponent<ArtefactInventory>());

        //Variables
        controller.CmdSetImmobilized(false);
        GetComponent<PlayerToPlayerInteraction>().CmdSetHasBeenStolenFrom(controller, false);
    }

    public void UpdateSetup(PlayerController controller)
    {
        //Sets up player name for scoreboard use and floating name use
        if (controller.playerNameText == null && SceneManager.GetActiveScene().name == "GameScene")
        {
            controller.playerNameText = Instantiate(Resources.Load<GameObject>("PlayerAssets/PlayerNameText_UI"));
            controller.playerNameText.transform.SetParent(FindObjectOfType<CanvasUIManager>().playerTextContainer.transform);
            controller.playerNameText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
            controller.playerNameText.SetActive(true);
            controller.playerNameText.GetComponent<Text>().text = controller.playerName;
        }
    }

    [Client]
    void SetCamera()
    {
        PlayerController controller = GetComponent<PlayerController>();
        //Currently what it's trying to do is find cameras, if it can't find cameras, it will instantiate cameras
        //It also modifies so many things. It is simply a mess that needs to be fixed, it's not worth commenting here
        controller.vCam = FindObjectOfType<Cinemachine.CinemachineFreeLook>();
        //DontDestroyOnLoad(vCam);
        controller.vCam.LookAt = this.gameObject.transform;
        controller.vCam.Follow = this.gameObject.transform;
        //vCam.transform.rotation = Quaternion.Euler(45, 0, 0);
        controller.playerCamera = Camera.main;
        if (!hasAuthority)
        {
            //Disable other players cameras so that we don't accidentally get assigned to another players camera
            if (controller.playerCamera != null)
                controller.playerCamera.gameObject.SetActive(false);
        }
        if (controller.devCam == null)
        {
            //REMINDER, you can't find the object if they are not in the same section ie dontdestroysection
            controller.devCam = GameObject.Find("DevCam");
            controller.devCam.SetActive(false);
        }
        else
        {
            controller.devCam = Instantiate(controller.devCam);
            controller.devCam.SetActive(false);
        }
    }

}
