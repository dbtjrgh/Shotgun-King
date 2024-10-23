using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CChessman : MonoBehaviour
{
    #region 변수
    public CUIDamagePool damagePool; // 데미지 UI 풀
    public int CurrentX { get; private set; }
    public int CurrentY { get; private set; }
    public bool isWhite; // 팀 구별
    public int health; // 기물 전체 체력
    public int currentHealth; // 기물 현재 체력

    public CCameraTransView cameraTransView;
    public GameObject Status; // 상태 UI 게임오브젝트
    public GameObject chessHp; // HP UI 게임오브젝트
    public GameObject heartPrefab; // HP 하트 프리팹
    public GameObject emptyHeartPrefab; // HP 빈하트 프리팹
    #endregion

    public Material originalMaterial { get; set; }

    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }

    public virtual bool[,] PossibleMove()
    {
        return new bool[8, 8];
    }
}
