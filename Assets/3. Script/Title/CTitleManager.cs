using System.Collections;
using System.Collections.Generic;
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

        titleUI.SetActive(true);
        gameStartUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
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
