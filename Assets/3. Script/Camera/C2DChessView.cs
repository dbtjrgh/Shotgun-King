using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class C2DChessView : MonoBehaviour
{
    private CCameraTransView cameraTransView;
    private RawImage image;
    private bool chessView;

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
        image = GetComponent<RawImage>();
        chessView = false;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !cameraTransView.isInTopView)
        {
            if(image.enabled)
            {
                image.enabled = false;
            }
            else if(!image.enabled)
            {
                image.enabled = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && cameraTransView.isInTopView)
        {
            image.enabled = false;
        }
        else if(Input.GetKeyDown(KeyCode.Space) && !cameraTransView.isInTopView)
        {
            image.enabled = true;
        }
    }
}
