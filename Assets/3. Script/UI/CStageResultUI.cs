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

    public GameObject rookImage;
    public GameObject bishopImage;
    public GameObject queenImage;

    private bool previousResultUIState = false; // resultUI�� ���� Ȱ��ȭ ����

    private void Awake()
    {
        boardManager = FindObjectOfType<CBoardManager>();
        DamageButton.onClick.AddListener(OnDamageButtonClick);
        RangeButton.onClick.AddListener(OnRangeButtonClick);
        ShotAngleButton.onClick.AddListener(OnShotAngleButtonClick);
    }

    private void Update()
    {
        if (playerShooting == null)
        {
            playerShooting = FindObjectOfType<CPlayerShooting>();
        }

        StageInfoTextUpdate();

        // resultUI�� ���°� ����Ǿ��� ���� Ŀ�� ���¸� ������Ʈ
        if (resultUI.activeSelf != previousResultUIState)
        {
            if (resultUI.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            previousResultUIState = resultUI.activeSelf; // ���� ���¸� ���� ���·� ����
        }
    }

    private void StageInfoTextUpdate()
    {
        StageInfoText.text = $"�������� {boardManager.stageFloor - 1} <color=green>Ŭ����!";
        switch (boardManager.stageFloor)
        {
            case 2:
                StageAddChessText.text = $"���� ��������\r\n�� 1�� �߰�\r\n�� 1�� �߰�";
                rookImage.gameObject.SetActive(true);
                bishopImage.gameObject.SetActive(false);
                queenImage.gameObject.SetActive(false);
                break;
            case 3:
                StageAddChessText.text = $"���� ��������\r\n��� 1�� �߰�\r\n����Ʈ 1�� �߰�\r\n�� 1�� �߰�";
                rookImage.gameObject.SetActive(false);
                bishopImage.gameObject.SetActive(true);
                queenImage.gameObject.SetActive(false);
                break;
            case 4:
                StageAddChessText.text = $"���� ��������\r\n�� 1�� �߰�\r\n�� 1�� �߰�";
                rookImage.gameObject.SetActive(false);
                bishopImage.gameObject.SetActive(false);
                queenImage.gameObject.SetActive(true);
                break;
            case 5:
                StageAddChessText.text = $"���� ��������\r\n�� 1�� �߰�\r\n�� 1�� �߰�";
                rookImage.gameObject.SetActive(true);
                bishopImage.gameObject.SetActive(false);
                queenImage.gameObject.SetActive(false);
                break;
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
