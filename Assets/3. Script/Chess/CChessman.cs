using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CChessman : MonoBehaviour
{
    #region 변수
    public CUIDamagePool damagePool; // 데미지 UI 풀
    public int CurrentX { get; private set; }
    public int CurrentY { get; private set; }
    public bool isWhite;

    public CCameraTransView cameraTransView;
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
