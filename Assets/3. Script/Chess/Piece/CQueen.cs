using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CQueen : CChessman
{
    public int health = 5;
    public int CurrentHealth;
    private Rigidbody rb;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody ����
        CurrentHealth = health;
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
        int i, j;

        // Right (���� �̵�ó�� ����)
        i = CurrentX;
        while (true)
        {
            i++;
            if (i >= 8)
                break;

            c = CBoardManager.instance.Chessmans[i, CurrentY];
            if (c == null)
            {
                r[i, CurrentY] = true;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    r[i, CurrentY] = true;
                }
                break; // �ٸ� ���� ������ ��� ����
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
                if (c.isWhite != isWhite)
                {
                    r[i, CurrentY] = true;
                }
                break;
            }
        }

        // Up
        j = CurrentY;
        while (true)
        {
            j++;
            if (j >= 8)
                break;

            c = CBoardManager.instance.Chessmans[CurrentX, j];
            if (c == null)
            {
                r[CurrentX, j] = true;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    r[CurrentX, j] = true;
                }
                break;
            }
        }

        // Down
        j = CurrentY;
        while (true)
        {
            j--;
            if (j < 0)
                break;

            c = CBoardManager.instance.Chessmans[CurrentX, j];
            if (c == null)
            {
                r[CurrentX, j] = true;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    r[CurrentX, j] = true;
                }
                break;
            }
        }

        // Top Left (����� �̵�ó�� �밢��)
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i--;
            j++;
            if (i < 0 || j >= 8)
                break;

            c = CBoardManager.instance.Chessmans[i, j];
            if (c == null)
            {
                r[i, j] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[i, j] = true;
                }
                break;
            }
        }

        // Top Right
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i++;
            j++;
            if (i >= 8 || j >= 8)
                break;

            c = CBoardManager.instance.Chessmans[i, j];
            if (c == null)
            {
                r[i, j] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[i, j] = true;
                }
                break;
            }
        }

        // Down Left
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i--;
            j--;
            if (i < 0 || j < 0)
                break;

            c = CBoardManager.instance.Chessmans[i, j];
            if (c == null)
            {
                r[i, j] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[i, j] = true;
                }
                break;
            }
        }

        // Down Right
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i++;
            j--;
            if (i >= 8 || j < 0)
                break;

            c = CBoardManager.instance.Chessmans[i, j];
            if (c == null)
            {
                r[i, j] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[i, j] = true;
                }
                break;
            }
        }

        return r;
    }
}
