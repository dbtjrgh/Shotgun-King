using System.Collections;
using UnityEngine;

public class CKing : CChessman
{
    public int health = 8; // ŷ�� ü��

    private Rigidbody rb; // ŷ�� Rigidbody ����
    private CCameraTransView cameraTransView;
    private bool isDead = false;

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody ����
    }

    private void Update()
    {
        if (!isWhite && cameraTransView.isInTopView)
        {
            return;
        }
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
        health--; // ü�� 1 ����

        if (health <= 0)
        {
            StartCoroutine(Die(collision)); // ü���� 0�̸� �״� ó��
        }
    }

    // ���� �� ���ư��� ����� �ı� ó��
    private IEnumerator Die(Collision collision)
    {
        isDead = true; // �̹� ���� ���·� ǥ��
        rb.isKinematic = false; // ���� ȿ�� ����

        // ������ �浹 �������� ���ư��� ����
        Vector3 knockbackDirection = collision.contacts[0].normal * -1; // �浹 ������ �ݴ� ����
        rb.AddForce(knockbackDirection * 500f); // ���� �־� ���ư��� ��

        yield return new WaitForSeconds(5f); // 5�� ��� ��

        CBoardManager.instance.EndGame(); // ü���� 0�� �Ǹ� ���� ���� ȣ��
    }

    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        CChessman c;
        int i, j;

        // Top Side
        i = CurrentX - 1;
        j = CurrentY + 1;
        if (CurrentY != 7)
        {
            for (int k = 0; k < 3; k++)
            {
                if (i != -1 && j != 8 && i != 8)
                {
                    c = CBoardManager.instance.Chessmans[i, j];

                    if (c == null)
                    {
                        r[i, j] = true;
                    }
                    else if (isWhite != c.isWhite)
                    {
                        r[i, j] = true;
                    }
                }
                i++;
            }
        }

        // Down Side
        i = CurrentX - 1;
        j = CurrentY - 1;
        if (CurrentY != 0)
        {
            for (int k = 0; k < 3; k++)
            {
                if (i != -1 && j != -1 && i != 8)
                {
                    c = CBoardManager.instance.Chessmans[i, j];
                    if (c == null)
                    {
                        r[i, j] = true;
                    }
                    else if (isWhite != c.isWhite)
                    {
                        r[i, j] = true;
                    }
                }
                i++;
            }
        }

        // Middle Left
        if (CurrentX - 1 != -1)
        {
            c = CBoardManager.instance.Chessmans[CurrentX - 1, CurrentY];
            if (c == null)
            {
                r[CurrentX - 1, CurrentY] = true;
            }
            else if (isWhite != c.isWhite)
            {
                r[CurrentX - 1, CurrentY] = true;
            }
        }

        // Middle Right
        if (CurrentX + 1 != 8)
        {
            c = CBoardManager.instance.Chessmans[CurrentX + 1, CurrentY];
            if (c == null)
            {
                r[CurrentX + 1, CurrentY] = true;
            }
            else if (isWhite != c.isWhite)
            {
                r[CurrentX + 1, CurrentY] = true;
            }
        }

        return r;
    }
}
