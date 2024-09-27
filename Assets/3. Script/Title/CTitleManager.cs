using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CTitleManager : MonoBehaviour
{
    #region º¯¼ö
    public Button gameStartButton;
    #endregion

    private void Awake()
    {
        gameStartButton.onClick.AddListener(OnStartButtonClick);
    }

    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("Forest 1~3 Stage");
    }

}
