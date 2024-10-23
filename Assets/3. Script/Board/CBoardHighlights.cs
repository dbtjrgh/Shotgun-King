using System.Collections.Generic;
using UnityEngine;

public class CBoardHighlights : MonoBehaviour
{
    #region 변수
    public static CBoardHighlights instance { get; set; }

    public GameObject highlightPrefab;
    private List<GameObject> highlights;
    #endregion

    void Awake()
    {
        instance = this;
        highlights = new List<GameObject>();
        InitializeHighlights(); // 강조 표시 초기화
    }

    /// <summary>
    /// 강조 표시를 초기화하는 함수
    /// </summary>
    void InitializeHighlights()
    {
        HideHighlights(); // 이전의 강조 표시 숨기기
        highlights.Clear(); // 이전 하이라이트 오브젝트 리스트 비우기
    }

    /// <summary>
    /// 강조된 이동 위치를 표시하는 함수
    /// </summary>
    /// <param name="moves"></param>
    public void HighlightAllowedMoves(bool[,] moves)
    {
        // instance가 유효한지 확인 후 강조 작업 수행
        if (CBoardHighlights.instance != null)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (moves[i, j])
                    {
                        HighlightTile(i, j); // 가능한 이동 위치에 강조 표시
                    }
                }
            }
        }
    }

    /// <summary>
    /// 타일 강조 표시
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void HighlightTile(int x, int y)
    {
        GameObject go = GetHighlightObject();
        go.SetActive(true);
        go.transform.position = new Vector3(x + 0.5f, 0.01f, y + 0.5f);
    }

    /// <summary>
    /// 강조 표시 오브젝트를 가져옴
    /// </summary>
    /// <returns></returns>
    GameObject GetHighlightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);

        if (go == null)
        {
            // highlightPrefab을 현재 오브젝트의 자식으로 생성
            go = Instantiate(highlightPrefab, this.transform);
            highlights.Add(go);
        }
        else
        {
            go.SetActive(true); // 사용 가능한 오브젝트를 활성화
        }

        return go;
    }

    /// <summary>
    /// 강조 표시 숨기기
    /// </summary>
    public void HideHighlights()
    {
        foreach (GameObject go in highlights)
        {
            go.SetActive(false);
        }
    }
}
