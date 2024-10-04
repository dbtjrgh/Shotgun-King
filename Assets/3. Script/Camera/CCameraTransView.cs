using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CCameraTransView : MonoBehaviour
{
    #region 변수
    [SerializeField]
    public CinemachineVirtualCamera topViewCinemachine; // 탑뷰 카메라
    public CinemachineVirtualCamera playerCinemachine; // 플레이어 카메라

    // 탑뷰 카메라가 활성화 되어 있는지 여부
    public bool isInTopView;
    #endregion

    private void Start()
    {
        isInTopView = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 만약 playerCinemachine이 여전히 null이면, 오브젝트를 계속해서 찾는다.
        if (playerCinemachine == null)
        {
            TryFindPlayerCinemachine();
        }

        // 탑뷰 카메라가 활성화 되어 있고, Space 키를 누르면 플레이어 카메라로 돌아가기
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CameraTransView();
        }
    }

    /// <summary>
    /// PlayerKing 오브젝트에서 CinemachineVirtualCamera 찾기 시도
    /// </summary>
    private void TryFindPlayerCinemachine()
    {
        GameObject playerKing = GameObject.Find("Black King(Clone)");
        if (playerKing != null)
        {
            playerCinemachine = playerKing.GetComponentInChildren<CinemachineVirtualCamera>();
        }
    }

    /// <summary>
    /// 카메라 전환
    /// </summary>
    private void CameraTransView()
    {
        if (!isInTopView)
        {
            if (playerCinemachine == null)
            {
                return;
            }

            // 탑뷰 카메라가 비활성화면 활성화
            if (!topViewCinemachine.gameObject.activeSelf)
            {
                topViewCinemachine.gameObject.SetActive(true);
            }

            // 카메라 우선순위 변경 | 플레이어 -> 탑뷰
            topViewCinemachine.Priority = 10;
            playerCinemachine.Priority = 0;

            // 탑뷰 카메라 활성화
            isInTopView = true;
            // 커서 화면 내 고정
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (isInTopView)
        {
            if (playerCinemachine == null)
            {
                return;
            }
            // 카메라 우선순위 변경 | 탑뷰 -> 플레이어
            topViewCinemachine.Priority = 0;
            playerCinemachine.Priority = 10;

            // 탑뷰 카메라 비활성화
            isInTopView = false;
            // 커서 잠금 모드
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
