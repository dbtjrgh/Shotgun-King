using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPawn : CChessman
{
    public int health = 3;
    public int CurrentHealth;
    private Rigidbody rb;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 참조
        CurrentHealth = health;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Projectile 레이어에 속한 오브젝트와 충돌했을 때
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile") && !isDead)
        {
            TakeDamage(collision); // 데미지 처리 함수 호출
        }
    }
    // 데미지 처리
    private void TakeDamage(Collision collision)
    {
        CurrentHealth--; // 체력 1 감소

        if (CurrentHealth <= 0)
        {
            Die(collision); // 체력이 0이면 죽는 처리
        }
    }

    // 죽을 때 날아가는 연출과 파괴 처리
    private void Die(Collision collision)
    {
        isDead = true; // 이미 죽은 상태로 표시
        rb.isKinematic = false; // 물리 효과 적용

        // 마지막 충돌 방향으로 날아가는 연출
        Vector3 knockbackDirection = collision.contacts[0].normal * -1; // 충돌 방향의 반대 방향
        rb.AddForce(knockbackDirection * 500f); // 힘을 주어 날아가게 함
        Destroy(gameObject, 1f);
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
                    r[i - 1, j + 1 ] = true;
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
