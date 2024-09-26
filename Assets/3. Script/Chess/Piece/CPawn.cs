using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPawn : CChessman
{
    public int health = 3;
    public int currentHealth;
    private Rigidbody rb;
    private bool isDead = false;
    public GameObject pawnStatus;
    public GameObject chessHp;
    public GameObject heartPrefab;
    public GameObject emptyHeartPrefab;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody ����
        currentHealth = health;
        if (pawnStatus != null)
        {
            pawnStatus.SetActive(false);
        }
        UpdateHealthUI();

    }

    private void UpdateHealthUI()
    {
        foreach (Transform child in chessHp.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < currentHealth; i++)
        {
            Instantiate(heartPrefab, chessHp.transform);
        }
        for (int i = 0; i < (health - currentHealth); i++)
        {
            Instantiate(emptyHeartPrefab, chessHp.transform);
        }
    }
    private void OnMouseEnter()
    {
        CChessUIManager.instance.ShowUI(pawnStatus);
    }
    private void OnMouseExit()
    {
        CChessUIManager.instance.HideUI(pawnStatus);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Projectile ���̾ ���� ������Ʈ�� �浹���� ��
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile") && !isDead)
        {
            TakeDamage(collision); // ������ ó�� �Լ� ȣ��
        }
    }
    // ������ ó��
    private void TakeDamage(Collision collision)
    {
        currentHealth--; // ü�� 1 ����
        UpdateHealthUI();
        if (currentHealth <= 0)
        {
            Die(collision); // ü���� 0�̸� �״� ó��
        }
    }

    // ���� �� ���ư��� ����� �ı� ó��
    private void Die(Collision collision)
    {
        isDead = true; // �̹� ���� ���·� ǥ��
        rb.isKinematic = false; // ���� ȿ�� ����

        // �浹�� �Ѿ��� ����� �ӵ��� ������� ���ư��� ����
        Vector3 knockbackDirection = collision.relativeVelocity.normalized; // �Ѿ��� ���ƿ� ����
        rb.AddForce(knockbackDirection * 50f, ForceMode.Impulse); // ���� ���� ���ư��� ��

        Destroy(gameObject, 1.5f); // 1�� �� ������Ʈ �ı�
    }

    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        CChessman c, c2;
        int i, j;
        int[] e = CBoardManager.instance.EnPassantMove;

        i = CurrentX;
        j = CurrentY;

        // White team move
        if (isWhite)
        {
            // Diagonal Left
            if (i != 0 && j != 7)
            {
                if (e[0] == i - 1 && e[1] == j + 1)
                {
                    r[i - 1, j + 1] = true;
                }
                c = CBoardManager.instance.Chessmans[i - 1, j + 1];
                if (c != null && !c.isWhite)
                {
                    r[i - 1, j + 1] = true;
                }
            }

            // Diagonal Right
            if (i != 7 && j != 7)
            {
                if (e[0] == i + 1 && e[1] == j + 1)
                {
                    r[i - 1, j + 1] = true;
                }
                c = CBoardManager.instance.Chessmans[i + 1, j + 1];
                if (c != null && !c.isWhite)
                {
                    r[i + 1, j + 1] = true;
                }
            }

            // Middle
            if (j != 7)
            {
                c = CBoardManager.instance.Chessmans[i, j + 1];
                if (c == null)
                {
                    r[i, j + 1] = true;
                }
            }

            // Middle on first move
            if (j == 1)
            {
                c = CBoardManager.instance.Chessmans[i, j + 1];
                c2 = CBoardManager.instance.Chessmans[i, j + 2];
                if (c == null && c2 == null)
                {
                    r[i, j + 2] = true;
                }
            }
        }
        else
        {
            // Diagonal Left
            if (i != 0 && j != 0)
            {
                if (e[0] == i - 1 && e[1] == j - 1)
                {
                    r[i - 1, j - 1] = true;
                }
                c = CBoardManager.instance.Chessmans[i - 1, j - 1];
                if (c != null && c.isWhite)
                {
                    r[i - 1, j - 1] = true;
                }
            }

            // Diagonal Right
            if (i != 7 && j != 0)
            {
                if (e[0] == i + 1 && e[1] == j - 1)
                {
                    r[i + 1, j - 1] = true;
                }
                c = CBoardManager.instance.Chessmans[i + 1, j - 1];
                if (c != null && c.isWhite)
                {
                    r[i + 1, j - 1] = true;
                }
            }

            // Middle
            if (j != 0)
            {
                c = CBoardManager.instance.Chessmans[i, j - 1];
                if (c == null)
                {
                    r[i, j - 1] = true;
                }
            }

            // Middle on first move
            if (j == 6)
            {
                c = CBoardManager.instance.Chessmans[i, j - 1];
                c2 = CBoardManager.instance.Chessmans[i, j - 2];
                if (c == null && c2 == null)
                {
                    r[i, j - 2] = true;
                }
            }
        }


        return r;
    }
}
