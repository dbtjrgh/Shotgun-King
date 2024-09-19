using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CCameraTransView : MonoBehaviour
{
    #region ����
    [SerializeField]
    public CinemachineVirtualCamera topViewCinemachine; // ž�� ī�޶�
    public CinemachineVirtualCamera playerCinemachine; // �÷��̾� ī�޶�

    // ž�� ī�޶� Ȱ��ȭ �Ǿ� �ִ��� ����
    // ���� ���� ���� bool��
    public bool isInTopView = false;
    #endregion

    private void Update()
    {
        // ž�� ī�޶� Ȱ��ȭ �Ǿ� �ְ�, escŰ�� ������ �÷��̾� ī�޶�� ���ư���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CameraTransView();
        }
    }

    /// <summary>
    /// ī�޶� ��ȯ
    /// </summary>
    private void CameraTransView()
    {
        if (!isInTopView)
        {
            if (playerCinemachine == null)
            {
                return;
            }

            // ž�� ī�޶� ��Ȱ��ȭ�� Ȱ��ȭ
            if (!topViewCinemachine.gameObject.activeSelf)
            {
                topViewCinemachine.gameObject.SetActive(true);
            }

            // ī�޶� �켱���� ���� | �÷��̾� -> ž��
            topViewCinemachine.Priority = 10;
            playerCinemachine.Priority = 0;

            // ž�� ī�޶� Ȱ��ȭ
            isInTopView = true;
            // Ŀ�� ȭ�� �� ����
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (isInTopView)
        {
            if (playerCinemachine == null)
            {
                return;
            }
            // ī�޶� �켱���� ���� | ž�� -> �÷��̾�
            topViewCinemachine.Priority = 0;
            playerCinemachine.Priority = 10;

            // ž�� ī�޶� ��Ȱ��ȭ
            isInTopView = false;
            // Ŀ�� ��� ���
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
