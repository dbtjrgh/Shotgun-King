using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CTitleManager : MonoBehaviour
{
    #region 변수
    // TitleUI 관련
    public GameObject titleUI;
    public Button gameStartButton;
    public Button optionButton;
    public Button quitButton;


    // GameStartUI 관련
    public GameObject gameStartUI;
    public Button stageStartButton;
    public Button tutorialStartButton;
    public Button backButton;

    // OptionUI 관련
    public GameObject optionUI;
    public Button optionBackButton;
    // 깜빡이는 텍스트 관련
    public GameObject blinkingText; // 깜빡일 텍스트
    private bool isBlinking = true; // 깜빡임 상태
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

        titleUI.SetActive(false); // 초기에는 titleUI 비활성화
        gameStartUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;

        StartCoroutine(BlinkText()); // 텍스트 깜빡임 시작
    }
    private void Update()
    {
        // 아무 키나 누르면 titleUI 활성화
        if (isBlinking && Input.anyKeyDown)
        {
            CSoundManager.Instance.PlaySfx(0);
            isBlinking = false; // 깜빡임 중지
            StopCoroutine(BlinkText()); // 깜빡임 코루틴 중지
            titleUI.SetActive(true); // titleUI 활성화
            blinkingText.gameObject.SetActive(false); // 깜빡이는 텍스트 비활성화
        }
    }
    private IEnumerator BlinkText()
    {
        while (isBlinking)
        {
            blinkingText.gameObject.SetActive(!blinkingText.gameObject.activeSelf); // 텍스트 깜빡임
            yield return new WaitForSeconds(1f); // 1초 대기
        }
    }

    public void OnStartButtonClick()
    {
        CSoundManager.Instance.PlaySfx(1);
        gameStartUI.SetActive(true);
        titleUI.SetActive(false);
    }

    public void OnOptionButtonClick()
    {
        CSoundManager.Instance.PlaySfx(1);
        optionUI.SetActive(true);
    }

    public void OnQuitButtonClick()
    {
        CSoundManager.Instance.PlaySfx(2);
        Application.Quit();
    }


    public void OnStageStartButtonClick()
    {
        CSoundManager.Instance.PlaySfx(1);
        SceneManager.LoadScene("island1-2StageScene");
    }

    public void OnTutorialStartButtonClick()
    {
        CSoundManager.Instance.PlaySfx(1);
        SceneManager.LoadScene("TutorialScene");
    }

    public void OnBackButtonClick()
    {
        CSoundManager.Instance.PlaySfx(2);
        gameStartUI.SetActive(false);
        titleUI.SetActive(true);
    }

    public void OnOptionBackButton()
    {
        CSoundManager.Instance.PlaySfx(3);
        optionUI.SetActive(false);
    }

    public void PointerEnter()
    {
        CSoundManager.Instance.PlaySfx(7);
    }

}
