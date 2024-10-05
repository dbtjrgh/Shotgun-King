using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CStageDefeatUI : MonoBehaviour
{
    public Button yesButton;
    public Button noButton;

    public GameObject defeatUI;

    private bool previousDefeatUIState = false;
    private CBoardManager boardManager;

    private void Awake()
    {
        yesButton.onClick.AddListener(YesButtonClick);
        noButton.onClick.AddListener(NoButtonButtonClick);
        boardManager = FindAnyObjectByType<CBoardManager>();
    }

    private void Update()
    {
        // resultUI의 상태가 변경되었을 때만 커서 상태를 업데이트
        if (defeatUI.activeSelf != previousDefeatUIState)
        {
            if (defeatUI.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            previousDefeatUIState = defeatUI.activeSelf; // 현재 상태를 이전 상태로 저장
        }
    }

    private void YesButtonClick()
    {
        if (boardManager.isTutorial)
        {
            SceneManager.LoadScene("TutorialScene");
        }
        else
        {
            SceneManager.LoadScene("island1-2StageScene");
        }
    }

    private void NoButtonButtonClick()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
