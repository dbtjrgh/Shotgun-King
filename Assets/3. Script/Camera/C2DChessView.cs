using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class C2DChessView : MonoBehaviour
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
        else if(!cameraTransView.isInTopView)
        {
            image.enabled = true;
        }
    }
}
