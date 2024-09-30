using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CStageResultUI : MonoBehaviour
{
    CBoardManager boardManager;
    CPlayerShooting playerShooting;

    public GameObject resultUI;
    public TextMeshProUGUI StageInfoText;
    public TextMeshProUGUI StageAddChessText;

    public Button DamageButton;
    public Button RangeButton;
    public Button ShotAngleButton;



    private void Awake()
    {
        boardManager = FindObjectOfType<CBoardManager>();
        DamageButton.onClick.AddListener(OnDamageButtonClick);
        RangeButton.onClick.AddListener(OnRangeButtonClick);
        ShotAngleButton.onClick.AddListener(OnShotAngleButtonClick);
    }
    private void Update()
    {
        if(playerShooting == null)
        {   
            playerShooting = FindObjectOfType<CPlayerShooting>();
        }
        if(resultUI.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if(!resultUI.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void OnDamageButtonClick()
    {
        boardManager.shotgunDamage += 1;
        resultUI.SetActive(false);
    }
    public void OnRangeButtonClick()
    {
        boardManager.MaxshotgunDistance += 1;
        resultUI.SetActive(false);
    }
    public void OnShotAngleButtonClick()
    {
        boardManager.shotAngle -= 10;
        resultUI.SetActive(false);
    }
}
