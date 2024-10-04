using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CTitleManager : MonoBehaviour
{
    #region ����
    // TitleUI ����
    public GameObject titleUI;
    public Button gameStartButton;
    public Button optionButton;
    public Button quitButton;


    // GameStartUI ����
    public GameObject gameStartUI;
    public Button stageStartButton;
    public Button tutorialStartButton;
    public Button backButton;

    // OptionUI ����
    public GameObject optionUI;
    public Button optionBackButton;
    // �����̴� �ؽ�Ʈ ����
    public GameObject blinkingText; // ������ �ؽ�Ʈ
    private bool isBlinking = true; // ������ ����
    #endregion

    private void Awake()
    {
        gameStartButton.onClick.AddListener(OnStartButtonClick);
        optionButton.onClick.AddListener(OnOptionButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);

        stageStartButton.onClick.AddListener(OnStageStartButtonClick);
        tutorialStartButton.onClick.AddListener(OnTutorialStartButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);

        optionBackButton.onClick.AddListener(OnOptionBackButton);

        titleUI.SetActive(false); // �ʱ⿡�� titleUI ��Ȱ��ȭ
        gameStartUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;

        StartCoroutine(BlinkText()); // �ؽ�Ʈ ������ ����
    }
    private void Update()
    {
        // �ƹ� Ű�� ������ titleUI Ȱ��ȭ
        if (isBlinking && Input.anyKeyDown)
        {
            isBlinking = false; // ������ ����
            StopCoroutine(BlinkText()); // ������ �ڷ�ƾ ����
            titleUI.SetActive(true); // titleUI Ȱ��ȭ
            blinkingText.gameObject.SetActive(false); // �����̴� �ؽ�Ʈ ��Ȱ��ȭ
        }
    }
    private IEnumerator BlinkText()
    {
        while (isBlinking)
        {
            blinkingText.gameObject.SetActive(!blinkingText.gameObject.activeSelf); // �ؽ�Ʈ ������
            yield return new WaitForSeconds(1f); // 1�� ���
        }
    }

    public void OnStartButtonClick()
    {
        // SceneManager.LoadScene("Forest 1~3 Stage");
        gameStartUI.SetActive(true);
        titleUI.SetActive(false);
    }

    public void OnOptionButtonClick()
    {
        optionUI.SetActive(true);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }


    public void OnStageStartButtonClick()
    {
        SceneManager.LoadScene("Stage1-2Scene");
    }

    public void OnTutorialStartButtonClick()
    {

        SceneManager.LoadScene("TutorialScene");
    }

    public void OnBackButtonClick()
    {
        gameStartUI.SetActive(false);
        titleUI.SetActive(true);
    }

    public void OnOptionBackButton()
    {
        optionUI.SetActive(false);
    }

}
