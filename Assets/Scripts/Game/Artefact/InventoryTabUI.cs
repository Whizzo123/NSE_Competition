using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The individual artefact UI element, holds count and ui
/// </summary>
public class InventoryTabUI : MonoBehaviour
{
    [Header("Information")]
    [SerializeField][Tooltip("The amount of this artefact type")]private int personalCount;
    [Space]

    [Header("Visuals")]
    public Image itemImage;
    public Text itemCountText;

    /// <summary>
    /// Fills in the ui element with text and an image, and updates count
    /// </summary>
    public void Populate(string name, int count)
    {
        Debug.Log("Populate under name: " + name);
        string substringedName = name.Replace("(Clone)", "");
        Debug.Log("Populate under sub: " + substringedName);

        itemImage.sprite = Resources.Load("UI/" + substringedName, typeof(Sprite)) as Sprite;

        personalCount = count;
        itemCountText.text = "" + personalCount;
    }
    /// <summary>
    /// Edits the count and updates the text
    /// </summary>
    public void EditCount(int toAdd)
    {
        personalCount += toAdd;
        itemCountText.text = "" + personalCount;
    }

    public int GrabPersonalCount()
    {
        return personalCount;
    }

}
