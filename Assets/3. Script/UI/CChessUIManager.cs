using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CChessUIManager : MonoBehaviour
{
    #region ����
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
            atkText.text = $"���ݷ�\n{playerShooting.shotgunDamage}";
            rangeText.text = $"��Ÿ�\n{playerShooting.MinshotgunDistance}-{playerShooting.MaxshotgunDistance}";
            shotAngleText.text = $"�߻簢\n{playerShooting.shotAngle}��";
            atkToolTipText.text = $"���ݽ� �߻��ϴ� ����ü �� {playerShooting.shotgunDamage}��,\r\n�� ���� ����ü�� ���� ������ 1�� ���ظ� �ݴϴ�.";
            rangeToolTipText.text = $"{playerShooting.MinshotgunDistance} - {playerShooting.MaxshotgunDistance}ĭ ���� �ִ� �� ���� ���� �� �� �ֽ��ϴ�.";
            shotAngleToolTipText.text = $"���ݽ� {playerShooting.shotAngle}������ ź�� �����ϴ�.";
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
