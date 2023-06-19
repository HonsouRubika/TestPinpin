using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pinpin;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI treeDisplay;
    public TextMeshProUGUI ChoppingBoostLevel;

    [Header("Properties")]
    [SerializeField] float textModificationDelay = 0.1f;

    public void SetTreeAmount(float amount)
    {
        treeDisplay.text = amount.ToString();
    }
    public void ChangeTreeAmount(int newAmount)
    {
        StartCoroutine(TreeAmountTextModificationDelay(newAmount));
    }

    IEnumerator TreeAmountTextModificationDelay(int newAmount)
    {
        float previousValue = GameManager.Instance.WoodCount - newAmount;

        for (int i = 1; i <= newAmount; i++)
        {
            treeDisplay.text = (previousValue + i).ToString();
            yield return new WaitForSeconds(textModificationDelay);
        }
    }

    #region Upgrades

    //called by button OnClick() event
    public void BuyChoppingUpgrade()
    {
        if (GameManager.Instance.UseWood(10))
        {
            //sound feedback
            GameManager.Instance.AudioManager.UpgradeButtonClickSFX();

            Debug.Log("Buy chopping speed upgrade");
            GameManager.Instance.Player.AddChoppingSpeedBoost();
        }
    }

    #endregion
}
