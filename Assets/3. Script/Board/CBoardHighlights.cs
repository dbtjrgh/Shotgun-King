using System.Collections.Generic;
using UnityEngine;

public class CBoardHighlights : MonoBehaviour
{
    #region ����
    public static CBoardHighlights instance { get; set; }

    public GameObject highlightPrefab;
    private List<GameObject> highlights;
    #endregion

    private void Awake()
    {
        instance = this;
        highlights = new List<GameObject>();
        InitializeHighlights(); // ���� ǥ�� �ʱ�ȭ
    }

    // ���� ǥ�ø� �ʱ�ȭ�ϴ� �Լ�
    private void InitializeHighlights()
    {
        HideHighlights(); // ������ ���� ǥ�� �����
        highlights.Clear(); // ���� ���̶���Ʈ ������Ʈ ����Ʈ ����
    }

    // ������ �̵� ��ġ�� ǥ���ϴ� �Լ�
    public void HighlightAllowedMoves(bool[,] moves)
    {
        // instance�� ��ȿ���� Ȯ�� �� ���� �۾� ����
        if (CBoardHighlights.instance != null)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (moves[i, j])
                    {
                        HighlightTile(i, j); // ������ �̵� ��ġ�� ���� ǥ��
                    }
                }
            }
        }
    }

    // Ÿ�� ���� ǥ��
    private void HighlightTile(int x, int y)
    {
        GameObject go = GetHighlightObject();
        go.SetActive(true);
        go.transform.position = new Vector3(x + 0.5f, 0.01f, y + 0.5f);
    }

    // ���� ǥ�� ������Ʈ�� ������
    private GameObject GetHighlightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);

        if (go == null)
        {
            // highlightPrefab�� ���� ������Ʈ�� �ڽ����� ����
            go = Instantiate(highlightPrefab, this.transform);
            highlights.Add(go);
        }
        else
        {
            go.SetActive(true); // ��� ������ ������Ʈ�� Ȱ��ȭ
        }

        return go;
    }

    // ���� ǥ�� �����
    public void HideHighlights()
    {
        foreach (GameObject go in highlights)
        {
            go.SetActive(false);
        }
    }

    // Ư�� ��ġ�� ������ �̵� ��ġ���� Ȯ���ϴ� �޼���
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
