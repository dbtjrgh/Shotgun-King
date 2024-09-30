using I18N.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRook : CChessman
{
    #region ����
    public int health = 5;
    public int currentHealth;
    private Rigidbody rb;
    private bool isDead = false;
    public GameObject rookStatus;
    public GameObject chessHp;             
    public GameObject heartPrefab;   
    public GameObject emptyHeartPrefab;
    #endregion

    private void Awake()
    {
        damagePool = FindObjectOfType<CUIDamagePool>(); // ������ Ǯ ã��
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = health;
        if (rookStatus != null)
        {
            rookStatus.SetActive(false); 
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
        CChessUIManager.instance.ShowUI(rookStatus);
    }

    private void OnMouseExit()
    {
        CChessUIManager.instance.HideUI(rookStatus);
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

