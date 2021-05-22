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
        Debug.Log("Populate under name: " + name);
        string substringedName = name.Replace("(Clone)", "");
        Debug.Log("Populate under sub: " + substringedName);
        itemImage.sprite = Resources.Load("UI/" + substringedName, typeof(Sprite)) as Sprite;
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
