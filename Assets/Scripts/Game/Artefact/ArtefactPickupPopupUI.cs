using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Pop up text for picking artefacts
/// </summary>
public class ArtefactPickupPopupUI : MonoBehaviour
{
    public Text popupText;

    /// <summary>
    /// Pops up message for artefact pickup
    /// </summary>
    public void SetMessage(ItemArtefact artefact)
    {
        popupText.text = "You picked up a " + artefact.name.Replace("(Clone)", "") + " this is worth " + artefact.points + " points";
    }

}
