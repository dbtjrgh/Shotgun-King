using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CChessUIManager : MonoBehaviour
{
    #region 변수
    public static CChessUIManager instance;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI shotAngleText;
    public TextMeshProUGUI atkToolTipText;
    public TextMeshProUGUI rangeToolTipText;
    public TextMeshProUGUI shotAngleToolTipText;
    private CPlayerShooting playerShooting;
    #endregion
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (playerShooting == null)
        {
            playerShooting = FindObjectOfType<CPlayerShooting>();
        }
        else
        {
            atkText.text = $"공격력\n{playerShooting.shotgunDamage}";
            rangeText.text = $"사거리\n{playerShooting.MinshotgunDistance}-{playerShooting.MaxshotgunDistance}";
            shotAngleText.text = $"발사각\n{playerShooting.shotAngle}º";
            atkToolTipText.text = $"공격시 발사하는 투사체 수 {playerShooting.shotgunDamage}개,\r\n적 말이 투사체에 맞을 때마다 1의 피해를 줍니다.";
            rangeToolTipText.text = $"{playerShooting.MinshotgunDistance} - {playerShooting.MaxshotgunDistance}칸 내에 있는 적 말을 공격 할 수 있습니다.";
            shotAngleToolTipText.text = $"공격시 {playerShooting.shotAngle}º까지 탄이 퍼집니다.";
        }
    }

    public void ShowUI(GameObject uiElement)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(true);
        }
    }

    public void HideUI(GameObject uiElement)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(false); 
        }
    }
}
