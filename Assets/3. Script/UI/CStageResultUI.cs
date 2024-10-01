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

    private bool previousResultUIState = false; // resultUI의 이전 활성화 상태

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

        // resultUI의 상태가 변경되었을 때만 커서 상태를 업데이트
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
            previousResultUIState = resultUI.activeSelf; // 현재 상태를 이전 상태로 저장
        }
    }

    private void StageInfoTextUpdate()
    {
        StageInfoText.text = $"스테이지 {boardManager.stageFloor - 1} <color=green>클리어!";
        switch (boardManager.stageFloor)
        {
            case 2:
                StageAddChessText.text = $"다음 스테이지\r\n룩 1개 추가\r\n폰 1개 추가";
                rookImage.gameObject.SetActive(true);
                bishopImage.gameObject.SetActive(false);
                queenImage.gameObject.SetActive(false);
                break;
            case 3:
                StageAddChessText.text = $"다음 스테이지\r\n비숍 1개 추가\r\n나이트 1개 추가\r\n폰 1개 추가";
                rookImage.gameObject.SetActive(false);
                bishopImage.gameObject.SetActive(true);
                queenImage.gameObject.SetActive(false);
                break;
            case 4:
                StageAddChessText.text = $"다음 스테이지\r\n퀸 1개 추가\r\n폰 1개 추가";
                rookImage.gameObject.SetActive(false);
                bishopImage.gameObject.SetActive(false);
                queenImage.gameObject.SetActive(true);
                break;
            case 5:
                StageAddChessText.text = $"다음 스테이지\r\n룩 1개 추가\r\n폰 1개 추가";
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
