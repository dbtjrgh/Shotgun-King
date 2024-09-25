using I18N.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CChessUIManager : MonoBehaviour
{
    public static CChessUIManager instance;

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
