using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class InventoryTabUI : MonoBehaviour
{
    private int personalCount;

    public Text itemNameText;
    public Text itemCountText;

    public void AddText(string name, int count)
    {
        itemNameText.text = name;
        personalCount = count;
        itemCountText.text = "" + personalCount;
    }

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
