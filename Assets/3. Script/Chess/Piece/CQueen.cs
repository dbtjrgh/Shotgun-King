using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CQueen : CChessman
{
    #region 변수
    public int health = 5;
    public int currentHealth;
    private Rigidbody rb;
    private bool isDead = false;
    public GameObject queenStatus;
    public GameObject chessHp;
    public GameObject heartPrefab;
    public GameObject emptyHeartPrefab;
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 참조
        currentHealth = health;
        if (queenStatus != null)
        {
            queenStatus.SetActive(false);
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
        CChessUIManager.instance.ShowUI(queenStatus);
    }

    private void OnMouseExit()
    {
        CChessUIManager.instance.HideUI(queenStatus);
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
        currentHealth--; // 체력 1 감소
        UpdateHealthUI();
        if (currentHealth <= 0)
        {
            Die(collision); // 체력이 0이면 죽는 처리
        }
    }

    // 죽을 때 날아가는 연출과 파괴 처리
    private void Die(Collision collision)
    {
        isDead = true; // 이미 죽은 상태로 표시
        rb.isKinematic = false; // 물리 효과 적용

        // 충돌한 총알의 방향과 속도를 기반으로 날아가는 연출
        Vector3 knockbackDirection = collision.relativeVelocity.normalized; // 총알이 날아온 방향
        rb.AddForce(knockbackDirection * 50f, ForceMode.Impulse); // 힘을 가해 날아가게 함

        Destroy(gameObject, 1.5f); // 1초 후 오브젝트 파괴
    }
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        CChessman c;
        int i, j;

        // Right (룩의 이동처럼 직선)
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
                break; // 다른 말이 있으면 경로 차단
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

        // Top Left (비숍의 이동처럼 대각선)
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
