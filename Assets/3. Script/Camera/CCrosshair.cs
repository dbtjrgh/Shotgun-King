using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCrosshair : MonoBehaviour
{
    private CCameraTransView cameraTransView;
    private RawImage image;

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
        image = GetComponent<RawImage>();
    }
    private void Update()
    {

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
