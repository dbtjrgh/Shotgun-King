using I18N.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRook : CChessman
{
    public int health = 5;
    public int CurrentHealth;
    private Rigidbody rb;
    private bool isDead = false;
    public GameObject rookUI;
    public GameObject ChessHp;              // Reference to the ChessHp container with GridLayoutGroup
    public GameObject coloredHeartPrefab;   // Prefab for colored heart (full health)
    public GameObject emptyHeartPrefab;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody ����
        CurrentHealth = health;
        if (rookUI != null)
        {
            rookUI.SetActive(false);  // Make sure the UI is initially hidden
        }
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        // Clear any existing hearts in the ChessHp container
        foreach (Transform child in ChessHp.transform)
        {
            Destroy(child.gameObject);
        }

        // Add colored hearts for current health
        for (int i = 0; i < CurrentHealth; i++)
        {
            Instantiate(coloredHeartPrefab, ChessHp.transform);  // Create colored heart
        }

        // Add empty hearts for lost health
        for (int i = 0; i < (health - CurrentHealth); i++)
        {
            Instantiate(emptyHeartPrefab, ChessHp.transform);  // Create empty heart
        }
    }
    private void OnMouseEnter()
    {
        CChessUIManager.instance.ShowUI(rookUI);
    }

    private void OnMouseExit()
    {
        CChessUIManager.instance.HideUI(rookUI);
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
        CurrentHealth--; // ü�� 1 ����
        UpdateHealthUI();

        if (CurrentHealth <= 0)
        {
            Die(collision); // ü���� 0�̸� �״� ó��
        }
    }

    // ���� �� ���ư��� ����� �ı� ó��
    private void Die(Collision collision)
    {
        isDead = true; // �̹� ���� ���·� ǥ��
        rb.isKinematic = false; // ���� ȿ�� ����

        // ������ �浹 �������� ���ư��� ����
        Vector3 knockbackDirection = collision.contacts[0].normal * -1; // �浹 ������ �ݴ� ����
        rb.AddForce(knockbackDirection * 500f); // ���� �־� ���ư��� ��
        Destroy(gameObject, 1f);
    }
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        CChessman c;
        int i;

        // Right
        i = CurrentX;
        while (true)
        {
            i++;
            if (i >= 8)
                break;

            c = CBoardManager.instance.Chessmans[i, CurrentY];
            if (c == null)
            {
                r[i, CurrentY] = true;  // �� ĭ
            }
            else
            {
                if (isWhite != c.isWhite) // �ٸ� ���� ���� ��
                {
                    r[i, CurrentY] = true;
                }
                break; // ���� ������ ��� ����
            }
        }

        // Left
        i = CurrentX;
        while (true)
        {
            i--;
            if (i < 0)
                break;

            c = CBoardManager.instance.Chessmans[i, CurrentY];
            if (c == null)
            {
                r[i, CurrentY] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[i, CurrentY] = true;
                }
                break;
            }
        }

        // Up
        i = CurrentY;
        while (true)
        {
            i++;
            if (i >= 8)
                break;

            c = CBoardManager.instance.Chessmans[CurrentX, i];
            if (c == null)
            {
                r[CurrentX, i] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[CurrentX, i] = true;
                }
                break;
            }
        }

        // Down
        i = CurrentY;
        while (true)
        {
            i--;
            if (i < 0)
                break;

            c = CBoardManager.instance.Chessmans[CurrentX, i];
            if (c == null)
            {
                r[CurrentX, i] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[CurrentX, i] = true;
                }
                break;
            }
        }

        return r;
    }
}

