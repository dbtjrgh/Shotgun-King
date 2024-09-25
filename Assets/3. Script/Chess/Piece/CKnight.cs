using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKnight : CChessman
{
    public int health = 3;
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

        // UpLeft
        KnightMove(CurrentX - 1, CurrentY + 2, ref r);
        // UpRight
        KnightMove(CurrentX + 1, CurrentY + 2, ref r);

        // RightUp
        KnightMove(CurrentX + 2, CurrentY + 1, ref r);
        // RightDown
        KnightMove(CurrentX + 2, CurrentY - 1, ref r);

        // DownLeft
        KnightMove(CurrentX - 1, CurrentY - 2, ref r);
        // DownRight
        KnightMove(CurrentX + 1, CurrentY - 2, ref r);

        // LeftUp
        KnightMove(CurrentX - 2, CurrentY + 1, ref r);
        // LeftDown
        KnightMove(CurrentX - 2, CurrentY - 1, ref r);
        return r;
    }

    public void KnightMove(int x, int y, ref bool[,] r)
    {
        CChessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = CBoardManager.instance.Chessmans[x, y];
            if (c == null)
            {
                r[x, y] = true;
            }
            else if (isWhite != c.isWhite)
            {
                r[x, y] = true;
            }
        }
    }
    
}
