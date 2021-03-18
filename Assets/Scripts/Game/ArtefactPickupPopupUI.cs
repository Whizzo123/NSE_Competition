using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class ArtefactPickupPopupUI : MonoBehaviour
{

    public Text popupText;

    public void SetMessage(ItemArtefact artefact)
    {
        popupText.text = "You picked up a " + artefact.name.Replace("(Clone)", "") + " this is worth " + artefact.points + " points";
    }

}
