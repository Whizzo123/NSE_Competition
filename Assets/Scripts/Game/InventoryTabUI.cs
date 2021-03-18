using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class InventoryTabUI : MonoBehaviour
{
    private int personalCount;

    public Image itemImage;
    public Text itemCountText;

    public void Populate(string name, int count)
    {
        itemImage.sprite = Resources.Load("UI/" + name, typeof(Sprite)) as Sprite;
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
