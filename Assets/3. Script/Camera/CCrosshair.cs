using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCrosshair : MonoBehaviour
{
    #region º¯¼ö
    private CCameraTransView cameraTransView;
    private CPlayerShooting playerShooting;
    private RawImage image;
    #endregion

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
        image = GetComponent<RawImage>();
        playerShooting = FindObjectOfType<CPlayerShooting>();
    }
    private void Update()
    {
        if (playerShooting == null)
        {
            image.enabled = false;
        }

        if (cameraTransView.isInTopView)
        {
            image.enabled = false;
        }
        else if (!cameraTransView.isInTopView)
        {
            image.enabled = true;
        }
    }
}
