using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pinpin;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI treeDisplay;

    public void ChangeTreeAmount(int newAmount)
    {
        treeDisplay.text = newAmount.ToString();
    }

    #region Upgrades

    //called by button
    public void BuyChoppingUpgrade()
    {
        if (GameManager.Instance.UseWood(10))
        {
            GameManager.Instance.Player.AddChoppingSpeedBoost();
            Debug.Log("Buy chopping speed upgrade");
        }
    }

    #endregion
}
