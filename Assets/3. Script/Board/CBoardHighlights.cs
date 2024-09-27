using System.Collections.Generic;
using UnityEngine;

public class CBoardHighlights : MonoBehaviour
{
    #region 변수
    public static CBoardHighlights instance { get; set; }

    public GameObject highlightPrefab;
    private List<GameObject> highlights;
    #endregion

    private void Awake()
    {
        instance = this;
        highlights = new List<GameObject>();
    }

    // 강조된 이동 위치를 표시하는 함수
    public void HighlightAllowedMoves(bool[,] moves)
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

    // 타일 강조 표시
    private void HighlightTile(int x, int y)
    {
        GameObject go = GetHighlightObject();
        go.SetActive(true);
        go.transform.position = new Vector3(x + 0.5f, 0.01f, y + 0.5f);
    }

    // 강조 표시 오브젝트를 가져옴
    private GameObject GetHighlightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);

        if (go == null)
        {
            go = Instantiate(highlightPrefab);
            highlights.Add(go);
        }

        return go;
    }

    // 강조 표시 숨기기
    public void Hidehighlights()
    {
        foreach (GameObject go in highlights)
        {
            go.SetActive(false);
        }
    }

    // 특정 위치가 강조된 이동 위치인지 확인하는 메서드
    public bool IsPositionHighlighted(Vector3 position)
    {
        foreach (GameObject highlight in highlights)
        {
            if (highlight.activeSelf && Vector3.Distance(highlight.transform.position, position) < 0.5f)
            {
                return true;
            }
        }
        return false;
    }
}
