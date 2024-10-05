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
        // resultUI�� ���°� ����Ǿ��� ���� Ŀ�� ���¸� ������Ʈ
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
            previousDefeatUIState = defeatUI.activeSelf; // ���� ���¸� ���� ���·� ����
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
